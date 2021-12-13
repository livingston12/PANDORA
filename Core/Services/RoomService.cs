using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pandora.Core.Extensions;
using Pandora.Core.Interfaces;
using Pandora.Core.Migrations;
using Pandora.Core.Models.Entities;
using Pandora.Core.Models.Requests;
using Pandora.Core.Models.Responses;
using Pandora.Core.Utils;
using Pandora.Core.ViewModels;

namespace Pandora.Services
{
    public class RoomService : PandoraService, IRoomService
    {
        public RoomService(PandoraDbContext context)
           : base(context)
        {
        }
        public async Task<Response<RoomViewModel>> GetAsync(RoomRequest filter)
        {
            Check.NotNull(filter, nameof(filter));

            Response<RoomViewModel> result = null;
            var query = GetQuery(filter);
            var total = await query.CountAsync().ConfigureAwait(false);
            if (total > 0)
            {
                var queryPaging = MapToViewModel(query)
                        .Skip(filter.SkipSize)
                        .Take(filter.PageSize);
                result = new Response<RoomViewModel>()
                {
                    List = queryPaging,
                    PageIndex = filter.PageIndex,
                    PageSize = filter.PageSize,
                    Total = total
                };
            }

            return result;
        }

        private IQueryable<RoomsEntity> GetQuery(RoomRequest filter)
        {
            var query = dbContext
                            .Rooms
                            .Include(m => m.Tables)
                            .AsQueryable();

            query = ApplyFilters(query, filter);
            query = query.OrderBy(filter.Sort);

            return query;
        }

        private static IQueryable<RoomsEntity> ApplyFilters(IQueryable<RoomsEntity> query, RoomRequest filter)
        {
            IQueryable<RoomsEntity> filteredQuery = query;
            filteredQuery = filteredQuery.Where(filter);
            filteredQuery = ApplyRoomIdsFilters(filteredQuery, filter.RoomIds);
            return filteredQuery;
        }

        private static IQueryable<RoomsEntity> ApplyRoomIdsFilters(IQueryable<RoomsEntity> query, string RoomIds)
        {
            if (RoomIds.IsNotNullOrEmpty())
            {
                query = query.Where(m => RoomIds.Contains(m.RoomId.ToString()));
            }

            return query;
        }

        private IEnumerable<RoomViewModel> MapToViewModel(IEnumerable<RoomsEntity> query)
        {
            return query
                .Select(m => new RoomViewModel
                {
                    RoomId = m.RoomId,
                    Room = m.Room,
                    Description = m.Description,
                    Tables = m.Tables
                });
        }

        public async Task<Response<TableViewModel>> GetTablesAsync(int roomId)
        {
            var query = dbContext
                        .Tables
                        .AsQueryable();
            query = GetTableQuery(query, roomId);
            return await GetTablesAsync(query);
        }

        public static IQueryable<TablesEntity> GetTableQuery(IQueryable<TablesEntity> query, int roomId)
        {
            return query.Where(m => m.RoomId == roomId && m.Active);
        }
        public async Task<Response<TableViewModel>> GetTablesAsync(IQueryable<TablesEntity> query)
        {
            Response<TableViewModel> result = null;
            var total = await query.CountAsync().ConfigureAwait(false);
            if (total > 0)
            {
                var data = MapToViewModelTables(query);
                result = new Response<TableViewModel>()
                {
                    List = data,
                    PageIndex = 1,
                    PageSize = total,
                    Total = total
                };
            }
            return result;
        }
        private IEnumerable<TableViewModel> MapToViewModelTables(IQueryable<TablesEntity> query)
        {
            return query
                    .Select(m => new TableViewModel
                    {
                        TableId = m.TableId,
                        Table = m.Table,
                        Description = m.Description,
                        Active = m.Active,
                        RoomId = m.RoomId,
                        IsReserved = m.IsReserved
                    });
        }
    }

}
