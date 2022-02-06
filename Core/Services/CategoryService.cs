using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pandora.Core.Extensions;
using Pandora.Core.Interfaces;
using Pandora.Core.Migrations;
using Pandora.Core.Models;
using Pandora.Core.Models.Entities;
using Pandora.Core.Models.Requests;
using Pandora.Core.Models.Responses;
using Pandora.Core.Models.Results;
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
                            .Include(m => m.Menu)
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
                    MenuId = m.MenuId,
                    Menu = m.Menu
                });
        }

        public async Task<Response<CategoryViewModel>> GetSummaryAsync()
        {
            Response<CategoryViewModel> result = null;
            var query = dbContext.Categories.Include(m => m.Menu);
            var total = await query.CountAsync().ConfigureAwait(false);
            if (total > 0)
            {
                result = new Response<CategoryViewModel>()
                {
                    List = MapToViewModel(query),
                    PageIndex = 1,
                    PageSize = total,
                    Total = total
                };
            }

            return result;
        }

        public async Task<Result<CategoryResult>> CreateAsync(CategoryCreateRequest request)
        {
            Check.NotNull(request, nameof(request));

            var result = new Result<CategoryResult>();
            result = await Create(request);

            return result;
        }

        private async Task<Result<CategoryResult>> Create(CategoryCreateRequest request)
        {
            Result<CategoryResult> result = new Result<CategoryResult>();
            try
            {
                result = await SaveAsync(request).ConfigureAwait(false);
            }
            catch (TimeoutException)
            {
                result.StatusCode = "408";
                result.Message = new StringBuilder("El servicio tardo mas de lo esperado en ejecutar.").ToString();
            }
            catch (ArgumentNullException)
            {
                result.StatusCode = "400";
                result.Message = new StringBuilder("No se puede crear un plato en blanco.").ToString();
            }

            return result;
        }

        private async Task<Result<CategoryResult>> SaveAsync(CategoryCreateRequest request)
        {
            Result<CategoryResult> result = new Result<CategoryResult>();
            result.Data = new CategoryResult(new Dictionary<string, IEnumerable<string>>());
            result.Data.Category = new CategoryEntity();

            var category = await CategoryExists(request);

            if (category != null)
            {
                result.Data.Category = category;
                result.StatusCode = "204";
                result.Message = "La categoria ya existe";
            }
            else
            {
                var data = await SaveAsync(request, result.Data).ConfigureAwait(false);
                result.Data = data.dataResult;
                result.StatusCode = data.statusCode;
                result.Message = data.message;
            }

            return result;
        }

        private async Task<CategoryEntity> CategoryExists(CategoryCreateRequest request)
        {
            var category = request.Category.Trim();

            var result = await dbContext.Categories
                .Where(m => m.Category == category)
                .FirstOrDefaultAsync();

            return result;
        }

        private async Task<(CategoryResult dataResult, string statusCode, string message)> SaveAsync(CategoryCreateRequest request, CategoryResult CategoryResult)
        {
            (CategoryResult dataResult, string statusCode, string message) result = (CategoryResult, "", "");
            List<string> errors = await validateCategory(request);
            if (errors.Any())
            {
                result.dataResult.Errors.Add("1", errors);
                result.statusCode = "400";
            }
            else
            {
                var tran = await dbContext.Database.BeginTransactionAsync();
                try
                {

                    CategoryEntity category = CategoryRequestToEntity(request);
                    dbContext.Add(category);
                    await dbContext.SaveChangesAsync();

                    await tran.CommitAsync();
                    result.dataResult.Category = category;
                    result.statusCode = "201";
                    result.message = "Categoria insertada correctamente";
                }
                catch (Exception ex)
                {
                    await tran.RollbackAsync();
                    errors.Add($"Error al insertar categoria: {ex.Message}");
                    result.dataResult.Errors.Add("1", errors);
                    result.statusCode = "400";
                }

            }

            return result;
        }

        private async Task<List<string>> validateCategory(CategoryCreateRequest request)
        {
            List<string> errors = new List<string>();
            bool menuExists = await dbContext.Categories.Where(m => m.MenuId == request.MenuId).AnyAsync();

            if (request.Category.Length == 0)
            {
                errors.Add("La <b>categoria</b> es obligatoria");
            }
            if (!menuExists)
            {
                errors.Add("El <b>menu</b> no existe");
            }

            return errors;
        }

        private CategoryEntity CategoryRequestToEntity(CategoryCreateRequest request)
        {
            var result = new CategoryEntity()
            {
                Category = request.Category,
                MenuId = request.MenuId.Value,
                Menu = GetMenuEntity(request.MenuId)
            };

            return result;
        }

        private MenusEntity GetMenuEntity(int? menuId)
        {
            return dbContext.Menus.Where(m => m.MenuId == menuId).FirstOrDefault();
        }

    }
}
