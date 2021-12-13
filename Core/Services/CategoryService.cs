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
    public class CategoryService : PandoraService, ICategoriesService
    {
        public CategoryService(PandoraDbContext context)
           : base(context)
        {
        }
        public async Task<Response<CategoryViewModel>> GetAsync(CategoryRequest filter)
        {
            Check.NotNull(filter, nameof(filter));

            Response<CategoryViewModel> result = null;
            var query = GetQuery(filter);
            var total = await query.CountAsync().ConfigureAwait(false);
            if (total > 0)
            {
                var queryPaging = MapToViewModel(query)
                        .Skip(filter.SkipSize)
                        .Take(filter.PageSize);
                result = new Response<CategoryViewModel>()
                {
                    List = queryPaging,
                    PageIndex = filter.PageIndex,
                    PageSize = filter.PageSize,
                    Total = total
                };
            }

            return result;
        }

        private IQueryable<CategoryEntity> GetQuery(CategoryRequest filter)
        {
            var query = dbContext
                            .Categories
                            .AsQueryable();

            query = ApplyFilters(query, filter);
            query = query.OrderBy(filter.Sort);

            return query;
        }

        private static IQueryable<CategoryEntity> ApplyFilters(IQueryable<CategoryEntity> query, CategoryRequest filter)
        {
            IQueryable<CategoryEntity> filteredQuery = query;
            filteredQuery = filteredQuery.Where(filter);
            filteredQuery = ApplyCategoriesIdsFilters(filteredQuery, filter.CategoryIds);
            filteredQuery = ApplyMenuIdsFilters(filteredQuery, filter.MenuIds);
            return filteredQuery;
        }

        private static IQueryable<CategoryEntity> ApplyCategoriesIdsFilters(IQueryable<CategoryEntity> query, string CategoryIds)
        {
            if (CategoryIds.IsNotNullOrEmpty())
            {
                query = query.Where(p => CategoryIds.Contains(p.CategoryId.ToString()));
            }

            return query;
        }
        private static IQueryable<CategoryEntity> ApplyMenuIdsFilters(IQueryable<CategoryEntity> query, string MenusIds)
        {
            if (MenusIds.IsNotNullOrEmpty())
            {
                query = query.Where(p => MenusIds.Contains(p.MenuId.ToString()));
            }

            return query;
        }

        private static IEnumerable<CategoryViewModel> MapToViewModel(IEnumerable<CategoryEntity> query)
        {
            return query
                .Select(m => new CategoryViewModel
                {
                    CategoryId = m.CategoryId,
                    Category = m.Category,
                    MenuId = m.MenuId
                });
        }
    }

}
