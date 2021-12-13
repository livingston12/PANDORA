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
    public class TableService : PandoraService, ITableService
    {
        public TableService(PandoraDbContext context)
           : base(context)
        {
        }
        public async Task<Response<TableViewModel>> GetAsync(TableRequest filter)
        {
            Check.NotNull(filter, nameof(filter));

            Response<TableViewModel> result = null;
            var query = GetQuery(filter);
            var total = await query.CountAsync().ConfigureAwait(false);
            if (total > 0)
            {
                var queryPaging = MapToViewModel(query)
                        .Skip(filter.SkipSize)
                        .Take(filter.PageSize);
                result = new Response<TableViewModel>()
                {
                    List = queryPaging,
                    PageIndex = filter.PageIndex,
                    PageSize = filter.PageSize,
                    Total = total
                };
            }

            return result;
        }

        private IQueryable<TablesEntity> GetQuery(TableRequest filter)
        {
            var query = dbContext
                            .Tables
                            .AsQueryable();

            query = ApplyFilters(query, filter);
            query = query.OrderBy(filter.Sort);

            return query;
        }

        private static IQueryable<TablesEntity> ApplyFilters(IQueryable<TablesEntity> query, TableRequest filter)
        {
            IQueryable<TablesEntity> filteredQuery = query;
            filteredQuery = filteredQuery.Where(filter);
            filteredQuery = ApplyTableIdsFilters(filteredQuery, filter.TableIds);
            return filteredQuery;
        }

        private static IQueryable<TablesEntity> ApplyTableIdsFilters(IQueryable<TablesEntity> query, string TableIds)
        {
            if (TableIds.IsNotNullOrEmpty())
            {
                query = query.Where(p => TableIds.Contains(p.TableId.ToString()));
            }

            return query;
        }


        private static IEnumerable<TableViewModel> MapToViewModel(IEnumerable<TablesEntity> query)
        {
            return query
                .Select(m => new TableViewModel
                {
                    TableId = m.TableId,
                    Description = m.Description,
                    Table = m.Table,
                    Active = m.Active,
                    RoomId = m.RoomId,
                    IsReserved = m.IsReserved
                })
                .Where(m => m.Active);
        }

        public async Task<bool> ReservedAsync(int tableId)
        {
            var currentTable = await dbContext.Tables.Where(m => m.TableId == tableId).FirstOrDefaultAsync();
            bool isReserved = false;
            try
            {
                if (currentTable != null)
                {
                    isReserved = currentTable.IsReserved.HasValue ?
                    currentTable.IsReserved.Value : false;

                    if (!isReserved)
                    {
                        currentTable.IsReserved = true;
                        dbContext.Entry(currentTable).State = EntityState.Modified;
                        await dbContext.SaveChangesAsync();
                        isReserved = true;
                    }
                }
            }
            catch (System.Exception)
            {

            }


            return isReserved;
        }
    }

}
