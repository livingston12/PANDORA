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
    public class DishService : PandoraService, IDishService
    {
        public DishService(PandoraDbContext context)
           : base(context)
        {
        }
        public async Task<Response<DishViewModel>> GetAsync(DishRequest filter)
        {
            Check.NotNull(filter, nameof(filter));

            Response<DishViewModel> result = null;
            var query = GetQuery(filter);
            var total = await query.CountAsync().ConfigureAwait(false);
            if (total > 0)
            {
                var queryPaging = MapToViewModel(query)
                        .Skip(filter.SkipSize)
                        .Take(filter.PageSize);
                result = new Response<DishViewModel>()
                {
                    List = queryPaging,
                    PageIndex = filter.PageIndex,
                    PageSize = filter.PageSize,
                    Total = total
                };
            }

            return result;
        }

        private IQueryable<DishesEntity> GetQuery(DishRequest filter)
        {
            var query = dbContext
                            .Dishes
                            .Include(a=>a.Ingredients)
                            .AsQueryable();

            query = ApplyFilters(query, filter);
            query = query.OrderBy(filter.Sort);

            return query;
        }

        private static IQueryable<DishesEntity> ApplyFilters(IQueryable<DishesEntity> query, DishRequest filter)
        {
            IQueryable<DishesEntity> filteredQuery = query;
            filteredQuery = filteredQuery.Where(filter);
            filteredQuery = ApplyDishIdsFilters(filteredQuery, filter.DishIds);
            return filteredQuery;
        }

        private static IQueryable<DishesEntity> ApplyDishIdsFilters(IQueryable<DishesEntity> query, string DishIds)
        {
            if (DishIds.IsNotNullOrEmpty())
            {
                query = query.Where(p => DishIds.Contains(p.DishId.ToString()));
            }

            return query;
        }

        private static IEnumerable<DishViewModel> MapToViewModel(IEnumerable<DishesEntity> query)
        {
            return query
                .Select(m => new DishViewModel
                {
                    DishId = m.DishId,
                    Dish = m.Dish,
                    Description = m.Description,
                    ExpirationDate = m.ExpirationDate,
                    Image = m.Image,
                    CategoryId = m.CategoryId,
                    Quantity = m.Quantity,
                    Price = m.Price                    
                });
        }

    }

}
