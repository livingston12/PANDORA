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
    public class IngredientService : PandoraService, IIngredientService
    {
        public IngredientService(PandoraDbContext context)
           : base(context)
        {
        }

        public async Task<Response<IngredientViewModel>> GetAsync(IngredientRequest filter)
        {
            Check.NotNull(filter, nameof(filter));

            Response<IngredientViewModel> result = null;
            var query = GetQuery(filter);
            var total = await query.CountAsync().ConfigureAwait(false);
            if (total > 0)
            {
                var queryPaging = MapToViewModel(query)
                        .Skip(filter.SkipSize)
                        .Take(filter.PageSize);
                result = new Response<IngredientViewModel>()
                {
                    List = queryPaging,
                    PageIndex = filter.PageIndex,
                    PageSize = filter.PageSize,
                    Total = total
                };
            }

            return result;
        }

        private IQueryable<IngredientEntity> GetQuery(IngredientRequest filter)
        {
            var query = dbContext
                            .Ingredients
                            .AsQueryable();

            query = ApplyFilters(query, filter);
            query = query.OrderBy(filter.Sort);

            return query;
        }

        private static IQueryable<IngredientEntity> ApplyFilters(IQueryable<IngredientEntity> query, IngredientRequest filter)
        {
            IQueryable<IngredientEntity> filteredQuery = query;
            filteredQuery = filteredQuery.Where(filter);
            filteredQuery = ApplyFilters(filteredQuery, filter.IngredientIds);
            return filteredQuery;
        }

        private static IQueryable<IngredientEntity> ApplyFilters(IQueryable<IngredientEntity> query, string ingredientIds)
        {
            if (ingredientIds.IsNotNullOrEmpty())
            {
                query = query.Where(p => ingredientIds.Contains(p.IngredientId.ToString()));
            }

            return query;
        }
        private static IEnumerable<IngredientViewModel> MapToViewModel(IEnumerable<IngredientEntity> query)
        {
            return query
                .Select(m => new IngredientViewModel
                {
                    IngredientId = m.IngredientId,
                    Ingredient = m.Ingredient,
                    Price = m.Price,
                    Quantity = m.Quantity,
                    RestaurantId = m.RestaurantId
                });
        }

        public async Task<Result<IngredientResult>> CreateAsync(IngredientViewModel request)
        {
            Check.NotNull(request, nameof(request));

            var result = new Result<IngredientResult>();
            result = await Create(request);
            return result;

        }

        private async Task<Result<IngredientResult>> Create(IngredientViewModel request)
        {
            Result<IngredientResult> result = new Result<IngredientResult>();
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
                result.Message = new StringBuilder("No se puede crear una orden en blanco.").ToString();
            }

            return result;

        }
        private async Task<Result<IngredientResult>> SaveAsync(IngredientViewModel request)
        {
            Result<IngredientResult> result = new Result<IngredientResult>();
            result.Data = new IngredientResult(new Dictionary<string, IEnumerable<string>>());
            result.Data.Ingredient = new IngredientEntity();

            var ingredient = await IngredientExist(request);

            if (ingredient != null)
            {
                result.Data.Ingredient = ingredient;
                result.StatusCode = "204";
                result.Message = "El ingrediente ya existe insertado";
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
        private async Task<IngredientEntity> IngredientExist(IngredientViewModel request)
        {
            var ingredient = request.Ingredient.Trim();
            var result = await dbContext.Ingredients
                .Where(m => m.Ingredient == ingredient
                    && request.RestaurantId == m.RestaurantId)
                .FirstOrDefaultAsync();
            return result;
        }
        private async Task<(IngredientResult dataResult, string statusCode, string message)> SaveAsync(IngredientViewModel request, IngredientResult ingredientResult)
        {
            (IngredientResult dataResult, string statusCode,string message) result = (ingredientResult, "", "");
            List<string> errors = await validateIngredient(request);
            if (errors.Any())
            {
                result.dataResult.Errors.Add("1", errors);
                result.statusCode = "400";
            }
            else
            {
                try
                {
                    IngredientEntity ingredient = IngredientRequestToEntity(request);
                    dbContext.Add(ingredient);
                    await dbContext.SaveChangesAsync();
                    result.dataResult.Ingredient = ingredient;
                    result.statusCode = "201";
                    result.message = "Ingrediente insertado correctamente";
                }
                catch (Exception ex)
                {
                    errors.Add($"Error al insertar ingrediente: {ex.Message}");
                    result.dataResult.Errors.Add("1", errors);
                    result.statusCode = "400";
                }

            }

            return result;
        }

        private static IngredientEntity IngredientRequestToEntity(IngredientViewModel request)
        {
            IngredientEntity result = new IngredientEntity()
            {
                Ingredient = request.Ingredient,
                Price = request.Price,
                Quantity = request.Quantity,
                RestaurantId = request.RestaurantId
            };
            return result;
        }

        public async Task<List<string>> validateIngredient(IngredientViewModel request)
        {
            List<string> errors = new List<string>();
            var restaurant = await dbContext.Restaurants.Where(m => m.RestaurantId == request.RestaurantId).FirstOrDefaultAsync();

            if (request.Ingredient.Length == 0)
            {
                errors.Add("El <b>ingrediente</b> es obligatorio");
            }
            if (request.Quantity <= 0)
            {
                errors.Add("La <b>cantidad</b> tiene que ser mayor que cero");
            }
            if (request.Price <= 0)
            {
                errors.Add("El <b>precio</b> tiene que ser mayor que cero");
            }
            if (restaurant == null)
            {
                errors.Add("El <b>restaurante</b> no es valido");
            }

            return errors;
        }

    }
}