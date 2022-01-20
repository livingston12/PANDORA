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

        private IQueryable<IngredientEntity> GetQuery(IngredientRequest filter, bool isApplyFilter = true)
        {
            var query = dbContext
                            .Ingredients
                            .AsQueryable();
            if (isApplyFilter)
            {
                query = ApplyFilters(query, filter);
                query = query.OrderBy(filter.Sort);
            }

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

        public async Task<IEnumerable<IngredientViewModel>> GetSummaryAsync(int restaurantId)
        {
            Check.NotNull(restaurantId, nameof(restaurantId));

            IEnumerable<IngredientViewModel> result = null;
            var query = dbContext.Ingredients
                            .Where(m => m.RestaurantId == restaurantId);
            var total = await query.CountAsync().ConfigureAwait(false);

            if (total > 0)
            {
                result = query
                    .Select(m => new IngredientViewModel()
                    {
                        Ingredient = m.Ingredient,
                        IngredientId = m.IngredientId,
                        Price = m.Price,
                        Quantity = m.Quantity,
                        RestaurantId = m.RestaurantId
                    });
            }

            return result;
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
            (IngredientResult dataResult, string statusCode, string message) result = (ingredientResult, "", "");
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

        public async Task<UpdateResult> PutAsync(IngredientViewModel request)
        {
            Check.NotNull(request, nameof(request));

            UpdateResult result = await PutAsync(request.IngredientId, request);
            return result;
        }

        private async Task<UpdateResult> PutAsync(int ingredientId, IngredientViewModel request)
        {
            UpdateResult result = new UpdateResult(new Dictionary<string, IEnumerable<string>>());
            (string Index, List<string> errors) listErrors = await ValidateRequest(request);
            var ingredient = await dbContext.Ingredients
                .Where(x => x.IngredientId == ingredientId)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (ingredient == null)
            {
                listErrors.errors.Add("El ingrediente no existe");
            }
            else if (!request.Quantity.HasValue)
            {
                listErrors.errors.Add("La <b>existencia</b> del ingrediente es obligatoria");
            }
            else if (request.Quantity.Value <= 0)
            {
                listErrors.errors.Add("La <b>existencia</b> debe ser <b>mayor</b> que <b>cero</b>");
            }

            if (listErrors.errors.Any())
            {
                result.Errors.Add(listErrors.Index, listErrors.errors);
                result.StatusCode = "400";
            }
            else
            {
                try
                {
                    ingredient.Ingredient = request.Ingredient;
                    ingredient.Price = request.Price;
                    ingredient.Quantity = request.Quantity;

                    dbContext.Entry(ingredient).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();
                }
                catch (System.Exception ex)
                {
                    listErrors.errors = new List<string>();
                    listErrors.errors.Add($"Error inesperado: ${ex.Message}");
                    result.Errors.Add(listErrors.Index, listErrors.errors);
                    result.StatusCode = "400";
                }
            }
            return result;
        }

        private async Task<(string Index, List<string> errors)> ValidateRequest(IngredientViewModel request)
        {
            (string Index, List<string> errors) results = ("1", new List<string>());
            List<string> errors = new List<string>();
            var restaurant = await dbContext.Ingredients
                .Where(x =>
                    x.RestaurantId == request.RestaurantId &&
                    x.IngredientId == request.IngredientId
                )
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (request.Price < 0)
            {
                errors.Add("El precio no puede ser menor que cero");
            }
            if (request.Quantity <= 0)
            {
                errors.Add("La cantidad tiene que ser mayor que cero");
            }
            if (restaurant == null)
            {
                errors.Add("Este ingrediente no pertenece a este restaurante");
            }

            results.errors = errors;
            return results;
        }


    }
}