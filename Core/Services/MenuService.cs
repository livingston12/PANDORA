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
    public class MenuService : PandoraService, IMenuService
    {
        public MenuService(PandoraDbContext context)
           : base(context)
        {
        }
        public async Task<Response<MenuViewModel>> GetAsync(MenuRequest filter)
        {
            Check.NotNull(filter, nameof(filter));

            Response<MenuViewModel> result = null;
            var query = GetQuery(filter);
            var total = await query.CountAsync().ConfigureAwait(false);
            if (total > 0)
            {
                var queryPaging = MapToViewModel(query)
                        .Skip(filter.SkipSize)
                        .Take(filter.PageSize);
                result = new Response<MenuViewModel>()
                {
                    List = queryPaging,
                    PageIndex = filter.PageIndex,
                    PageSize = filter.PageSize,
                    Total = total
                };
            }

            return result;
        }

        private IQueryable<MenusEntity> GetQuery(MenuRequest filter)
        {
            var query = dbContext
                            .Menus
                            .AsQueryable();

            query = ApplyFilters(query, filter);
            query = query.OrderBy(filter.Sort);

            return query;
        }

        private static IQueryable<MenusEntity> ApplyFilters(IQueryable<MenusEntity> query, MenuRequest filter)
        {
            IQueryable<MenusEntity> filteredQuery = query;
            filteredQuery = filteredQuery.Where(filter);
            filteredQuery = ApplyMenuIdsFilters(filteredQuery, filter.MenuIds);
            return filteredQuery;
        }

        private static IQueryable<MenusEntity> ApplyMenuIdsFilters(IQueryable<MenusEntity> query, string MenuIds)
        {
            if (MenuIds.IsNotNullOrEmpty())
            {
                query = query.Where(p => MenuIds.Contains(p.MenuId.ToString()));
            }

            return query;
        }

        private static IEnumerable<MenuViewModel> MapToViewModel(IEnumerable<MenusEntity> query)
        {
            return query
                .Select(m => new MenuViewModel
                {
                    MenuId = m.MenuId,
                    Menu = m.Menu,
                    ExpirationDate = m.ExpirationDate,
                    RestaurantId = m.RestaurantId
                });
        }

        public async Task<Response<CategoryViewModel>> GetCategories(int menuId)
        {
            Response<CategoryViewModel> result = null;
            var query = dbContext
                            .Categories
                            .Where(x => x.MenuId == menuId)
                            .AsQueryable();

            var total = await query.CountAsync().ConfigureAwait(false);
            if (total > 0)
            {
                var data = MapToViewModelCategory(query);

                result = new Response<CategoryViewModel>()
                {
                    List = data,
                    PageIndex = 1,
                    PageSize = total,
                    Total = total
                };
            }

            return result;
        }

        private static IEnumerable<CategoryViewModel> MapToViewModelCategory(IEnumerable<CategoryEntity> query)
        {
            return query
                .Select(m => new CategoryViewModel
                {
                    CategoryId = m.CategoryId,
                    Category = m.Category,
                    MenuId = m.MenuId
                });
        }

        public async Task<Response<DishViewModel>> GetDishes(int categoryId)
        {
            Response<DishViewModel> result = null;
            var query = dbContext
                            .Dishes
                            .Where(x => x.CategoryId == categoryId)
                            .AsQueryable();

            var total = await query.CountAsync().ConfigureAwait(false);
            if (total > 0)
            {
                var data = MapToViewModelDish(query);

                result = new Response<DishViewModel>()
                {
                    List = data,
                    PageIndex = 1,
                    PageSize = total,
                    Total = total
                };
            }

            return result;
        }

        private static IEnumerable<DishViewModel> MapToViewModelDish(IQueryable<DishesEntity> query)
        {
            return query
                 .Select(m => new DishViewModel
                 {
                     CategoryId = m.CategoryId,
                     Description = m.Description,
                     Dish = m.Dish,
                     DishId = m.DishId,
                     ExpirationDate = m.ExpirationDate,
                     Image = m.Image,
                     Price = m.Price,
                     Quantity = m.Quantity
                 });
        }
    }

}
