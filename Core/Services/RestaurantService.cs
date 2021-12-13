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
    public class RestaurantService : PandoraService, IRestaurantService
    {
        public RestaurantService(PandoraDbContext context)
           : base(context)
        {
        }
        public async Task<Response<RestaurantViewModel>> GetAsync(RestaurantRequest filter)
        {
            Check.NotNull(filter, nameof(filter));

            Response<RestaurantViewModel> result = null;
            var query = GetQuery(filter);
            var total = await query.CountAsync().ConfigureAwait(false);
            if (total > 0)
            {
                var queryPaging = MapToViewModel(query)
                        .Skip(filter.SkipSize)
                        .Take(filter.PageSize);
                result = new Response<RestaurantViewModel>()
                {
                    List = queryPaging,
                    PageIndex = filter.PageIndex,
                    PageSize = filter.PageSize,
                    Total = total
                };
            }

            return result;
        }

        private IQueryable<RestaurantsEntity> GetQuery(RestaurantRequest filter)
        {
            var query = dbContext
                            .Restaurants
                            .AsQueryable();

            query = ApplyFilters(query, filter);
            query = query.OrderBy(filter.Sort);

            return query;
        }

        private static IQueryable<RestaurantsEntity> ApplyFilters(IQueryable<RestaurantsEntity> query, RestaurantRequest filter)
        {
            IQueryable<RestaurantsEntity> filteredQuery = query;
            filteredQuery = filteredQuery.Where(filter);
            filteredQuery = ApplyRestaurantIdsFilters(filteredQuery, filter.RestaurantIds);
            return filteredQuery;
        }

        private static IQueryable<RestaurantsEntity> ApplyRestaurantIdsFilters(IQueryable<RestaurantsEntity> query, string RestaurantIds)
        {
            if (RestaurantIds.IsNotNullOrEmpty())
            {
                query = query.Where(p => RestaurantIds.Contains(p.RestaurantId.ToString()));
            }

            return query;
        }

        private static IEnumerable<RestaurantViewModel> MapToViewModel(IEnumerable<RestaurantsEntity> query)
        {
            return query
                .Select(m => new RestaurantViewModel
                {
                    RestaurantId = m.RestaurantId,
                    Name = m.Name,
                    Address = m.Address,
                    Document = m.Document,
                    Image = m.Image,
                    Phone = m.Phone,

                });
        }

    }

}
