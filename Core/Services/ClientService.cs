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
    public class ClientService : PandoraService, IClientService
    {
        public ClientService(PandoraDbContext context)
          : base(context)
        {
        }
        public async Task<Response<ClientViewModel>> GetAsync(ClientRequest filter)
        {
            Check.NotNull(filter, nameof(filter));

            Response<ClientViewModel> result = null;
            var query = GetQuery(filter);
            var total = await query.CountAsync().ConfigureAwait(false);
            if (total > 0)
            {
                var queryPaging = MapToViewModel(query)
                        .Skip(filter.SkipSize)
                        .Take(filter.PageSize);
                result = new Response<ClientViewModel>()
                {
                    List = queryPaging,
                    PageIndex = filter.PageIndex,
                    PageSize = filter.PageSize,
                    Total = total
                };
            }

            return result;
        }

        private IQueryable<ClientsEntity> GetQuery(ClientRequest filter)
        {
            var query = dbContext
                            .Clients
                            .AsQueryable();

            query = ApplyFilters(query, filter);
            query = query.OrderBy(filter.Sort);

            return query;
        }

        private static IQueryable<ClientsEntity> ApplyFilters(IQueryable<ClientsEntity> query, ClientRequest filter)
        {
            IQueryable<ClientsEntity> filteredQuery = query;
            filteredQuery = filteredQuery.Where(filter);
            filteredQuery = ApplyClientIdsFilters(filteredQuery, filter.ClientIds);
            return filteredQuery;
        }


        private static IQueryable<ClientsEntity> ApplyClientIdsFilters(IQueryable<ClientsEntity> query, string clientIds)
        {
            if (clientIds.IsNotNullOrEmpty())
            {
                query = query.Where(m => clientIds.Contains(m.ClientId.ToString()));
            }

            return query;
        }

        private static IEnumerable<ClientViewModel> MapToViewModel(IEnumerable<ClientsEntity> query)
        {
            return query
                .Select(m => new ClientViewModel
                {
                    ClientId = m.ClientId,
                    Name = m.Name,
                    LastName = m.LastName,
                    Address = m.Address,
                    Phone = m.Phone
                });
        }
    }
}