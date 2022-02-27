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
                            .Include(a => a.Ingredients)
                            .Include(a => a.Category)
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
                    CategoryId = m.CategoryId,
                    Quantity = m.Quantity,
                    Price = m.Price,
                    NeedGarrison = m.NeedGarrison,
                    Category = m.Category,
                    Ingredients = m.Ingredients
                        .Select(x =>
                        new DishesDetailViewModel()
                        {
                            DishDetailId = x.DishDetailId,
                        })
                });
        }

        public async Task<Result<DishResult>> CreateAsync(DishViewModelCreate request)
        {
            Check.NotNull(request, nameof(request));

            var result = new Result<DishResult>();
            result = await Create(request);
            return result;

        }

        private async Task<Result<DishResult>> Create(DishViewModelCreate request)
        {
            Result<DishResult> result = new Result<DishResult>();
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
        private async Task<Result<DishResult>> SaveAsync(DishViewModelCreate request)
        {
            Result<DishResult> result = new Result<DishResult>();
            result.Data = new DishResult(new Dictionary<string, IEnumerable<string>>());
            result.Data.Dish = new DishesEntity();

            var dish = await DishExists(request);

            if (dish != null)
            {
                result.Data.Dish = dish;
                result.StatusCode = "204";
                result.Message = "El plato ya existe";
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
        private async Task<DishesEntity> DishExists(DishViewModelCreate request)
        {
            var dish = request.Dish.Trim();
            var result = await dbContext.Dishes
                .Where(m => m.Dish == dish)
                .FirstOrDefaultAsync();

            return result;
        }
        private async Task<(DishResult dataResult, string statusCode, string message)> SaveAsync(DishViewModelCreate request, DishResult dishResult)
        {
            (DishResult dataResult, string statusCode, string message) result = (dishResult, "", "");
            List<string> errors = await validateDish(request);
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

                    DishesEntity dish = DishRequestToEntity(request);
                    dbContext.Add(dish);
                    await dbContext.SaveChangesAsync();

                    IEnumerable<DishesDetailEntity> dishDetail = DishDetailRequestToEntity(dish.DishId, request.Ingredients);
                    dbContext.AddRange(dishDetail);
                    await dbContext.SaveChangesAsync();

                    await tran.CommitAsync();
                    result.dataResult.Dish = dish;
                    result.statusCode = "201";
                    result.message = "Plato insertado correctamente";
                }
                catch (Exception ex)
                {
                    await tran.RollbackAsync();
                    errors.Add($"Error al insertar plato: {ex.Message}");
                    result.dataResult.Errors.Add("1", errors);
                    result.statusCode = "400";
                }

            }

            return result;
        }

        private async Task<List<string>> validateDish(DishViewModelCreate request)
        {
            List<string> errors = new List<string>();
            bool categoryExist = await dbContext.Categories.Where(m => m.CategoryId == request.CategoryId).AnyAsync();
            bool ingredientExit = request.Ingredients.Any();

            if (request.Dish.Length == 0)
            {
                errors.Add("El <b>plato</b> es obligatorio");
            }
            if (request.Price <= 0)
            {
                errors.Add("El <b>precio</b> tiene que ser mayor que cero");
            }
            if (!categoryExist)
            {
                errors.Add("La <b>categoria</b> no existe");
            }
            if (request.NeedGarrison == true && ingredientExit)
            {
                errors.Add("El plato es una guarnicion no debe contener ingredientes");
            }
            else if (ingredientExit)
            {
                var errorDetail = await validateDishDetail(request.Ingredients);
                if (errorDetail.Any())
                {
                    errors.AddRange(errorDetail);
                }
            }
            else if (request.Quantity <= 0)
            {
                errors.Add("La <b>cantidad</b> es obligatoria");
            }

            return errors;
        }

        private async Task<List<string>> validateDishDetail(IEnumerable<DishDetailViewModel> ingredients)
        {
            List<string> errors = new List<string>();
            foreach (var ingre in ingredients)
            {
                bool existIngredient = await dbContext.Ingredients.Where(m => m.IngredientId == ingre.Ingredient.IngredientId).AnyAsync();
                if (ingre.QuantityRequired <= 0)
                {
                    errors.Add("La <b>cantidad</b> requerida debe ser <b>mayor</b> que <b>cero</b>");
                }
                if (ingre.Ingredient.Ingredient.Length <= 3)
                {
                    errors.Add($"El <b>ingrediente (${ingre.Ingredient.Ingredient})</b> debe contener mas de 3 caracteres");
                }
                if (ingre.Ingredient.Price <= 0)
                {
                    errors.Add("El <b>precio</b> del ingrediente debe ser <b>mayor</b> que <b>cero</b>");
                }
                if (!ingre.Ingredient.Quantity.HasValue)
                {
                    errors.Add("La <b>existencia</b> del ingrediente es obligatoria");
                }
                else if (ingre.Ingredient.Quantity.Value <= 0)
                {
                    errors.Add("La <b>existencia</b> debe ser <b>mayor</b> que <b>cero</b>");
                }

                if (!existIngredient)
                {
                    errors.Add("El ingrediente no existe");
                }
            }

            return errors;
        }

        private static DishesEntity DishRequestToEntity(DishViewModelCreate request)
        {
            DishesEntity result = new DishesEntity()
            {
                Dish = request.Dish,
                Price = request.Price,
                Quantity = request.Quantity,
                Description = request.Description,
                CategoryId = request.CategoryId,
                ExpirationDate = request.ExpirationDate,
                NeedGarrison = request.NeedGarrison
            };
            return result;
        }

        private static IEnumerable<DishesDetailEntity> DishDetailRequestToEntity(int dishId, IEnumerable<DishDetailViewModel> ingredients)
        {
            var result = ingredients.Select(m => new DishesDetailEntity()
            {
                DishId = dishId,
                IngredientId = m.Ingredient.IngredientId,
                Quantity = m.QuantityRequired
            });
            return result;
        }

        public async Task<IEnumerable<DishDetailViewModel>> GetDetailAsync(int dishId)
        {
            var result = await dbContext.DishDetails
                .Include(m => m.Ingredient)
                .Where(m => m.DishId == dishId)
                .Select(m => new DishDetailViewModel()
                {
                    DishDetailId = m.DishDetailId,
                    Ingredient = GetIngredientViewModel(m.Ingredient),
                    QuantityRequired = m.Quantity
                })
                .ToListAsync();
            return result;
        }

        public static IngredientViewModel GetIngredientViewModel(IngredientEntity ingredient)
        {
            IngredientViewModel result = new IngredientViewModel();
            if (ingredient != null)
            {
                result = new IngredientViewModel()
                {
                    Ingredient = ingredient.Ingredient,
                    IngredientId = ingredient.IngredientId,
                    Price = ingredient.Price,
                    Quantity = ingredient.Quantity,
                    RestaurantId = ingredient.RestaurantId
                };
            }
            return result;
        }

        public async Task<UpdateResult> PutAsync(DishUpdateViewModel request)
        {
            Check.NotNull(request, nameof(request));

            UpdateResult result = await PutAsync(request.DishId, request);
            return result;
        }

        private async Task<UpdateResult> PutAsync(int dishId, DishUpdateViewModel request)
        {
            UpdateResult result = new UpdateResult(new Dictionary<string, IEnumerable<string>>());
            (string Index, List<string> errors) listErrors = await ValidateRequest(request);
            var dish = await dbContext.Dishes.Where(x => x.DishId == dishId).FirstOrDefaultAsync().ConfigureAwait(false);

            if (dish == null)
            {
                listErrors.errors.Add("El plato no existe");
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
                    dish.Dish = request.Dish;
                    dish.Price = request.Price;
                    dish.Quantity = request.Quantity;
                    dish.CategoryId = request.CategoryId;
                    dish.Description = request.Description;
                    dish.NeedGarrison = request.NeedGarrison;
                    dbContext.Entry(dish).State = EntityState.Modified;
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

        private async Task<(string Index, List<string> errors)> ValidateRequest(DishUpdateViewModel request)
        {
            (string Index, List<string> errors) results = ("1", new List<string>());
            List<string> errors = new List<string>();
            var category = await dbContext.Categories.Where(x => x.CategoryId == request.CategoryId).FirstOrDefaultAsync().ConfigureAwait(false);

            if (request.Price <= 0)
            {
                errors.Add("El precio tiene que ser mayor que cero");
            }
            //if (request.Quantity <= 0)
            //{
                //errors.Add("La cantidad tiene que ser mayor que cero");
            //}
            if (category == null)
            {
                errors.Add("La categoria no existe");
            }
            results.errors = errors;
            return results;
        }

        public async Task<UpdateResult> PutDetailAsync(DishDetailRequest request)
        {
            Check.NotNull(request, nameof(request));

            UpdateResult result = await UpdateDetailAsync(request);
            return result;
        }

        private async Task<UpdateResult> UpdateDetailAsync(DishDetailRequest request)
        {
            UpdateResult result = new UpdateResult(new Dictionary<string, IEnumerable<string>>());
            (string Index, List<string> errors) listErrors = ValidateDetailRequest(request);
            var dishDetail = await dbContext.DishDetails.Where(x => x.DishDetailId == request.DishDetailId).FirstOrDefaultAsync();

            if (dishDetail == null)
            {
                listErrors.errors.Add("El detalle del plato no existe");
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
                    dishDetail.Quantity = request.QuantityRequired.Value;
                    dbContext.Entry(dishDetail).State = EntityState.Modified;
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

        private static (string Index, List<string> errors) ValidateDetailRequest(DishDetailRequest request)
        {
            (string Index, List<string> errors) results = ("1", new List<string>());
            List<string> errors = new List<string>();

            if (request.QuantityRequired <= 0)
            {
                errors.Add("La cantidad tiene que ser mayor que cero");
            }
            if (!request.QuantityRequired.HasValue)
            {
                errors.Add("La cantidad tiene que tener un valor");
            }
            results.errors = errors;
            return results;
        }

        public async Task<UpdateResult> DeleteAsync(int dishId)
        {
            var dish = await dbContext.Dishes
                .Where(x => x.DishId == dishId)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            List<string> dataError = new List<string>();
            UpdateResult result = new UpdateResult(new Dictionary<string, IEnumerable<string>>());

            try
            {
                if (dish == null)
                {
                    dataError.Add("El plato no existe");
                }
                else
                {
                    RemoveDishRelation(dishId);
                    dbContext.Remove(dish);
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                dataError.Add($"Error inesperado: {ex.Message}");
            }

            if (dataError.Any())
            {
                result.Errors.Add("1", dataError);
            }

            return result;
        }

        private void RemoveDishRelation(int dishId)
        {
            List<string> dataError = new List<string>();
            try
            {
                var dishDetails = dbContext.DishDetails
                   .Where(m => m.DishId == dishId);
                var orderIds = dbContext.OrdersDetail
                                    .Where(m => m.DishId == dishId)
                                    .Select(m => m.OrderId);

                var orderDetails = dbContext.OrdersDetail.Where(m => orderIds.Contains(m.OrderId));
                var orders = dbContext.Orders.Where(m => orderIds.Contains(m.OrderId));
                var invoices = dbContext.Invoices.Where(m => orderIds.Contains(m.OrderId));

                if (dishDetails.Any())
                {
                    dbContext.RemoveRange(dishDetails);
                }
                if (orderDetails.Any())
                {
                    dbContext.RemoveRange(orderDetails);
                }
                if (invoices.Any())
                {
                    dbContext.RemoveRange(invoices);
                }
                if (orders.Any())
                {
                    dbContext.RemoveRange(orders);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UpdateResult> DeleteDetailAsync(int dishDetailId)
        {
            var dishDetail = await dbContext.DishDetails
                .Where(x => x.DishDetailId == dishDetailId)
                .FirstOrDefaultAsync();

            List<string> dataError = new List<string>();
            UpdateResult result = new UpdateResult(new Dictionary<string, IEnumerable<string>>());

            if (dishDetail == null)
            {
                dataError.Add("El detalle del plato no existe");
            }
            else
            {
                dbContext.Remove(dishDetail);
                await dbContext.SaveChangesAsync();
            }

            if (dataError.Any())
            {
                result.Errors.Add("1", dataError);
            }

            return result;
        }

        public async Task<Result<DishDetailResult>> CreateDetailAsync(DishDetailViewModelCreate request)
        {
            Check.NotNull(request, nameof(request));

            var result = new Result<DishDetailResult>();
            result = await CreateDetail(request);
            return result;

        }

        private async Task<Result<DishDetailResult>> CreateDetail(DishDetailViewModelCreate request)
        {
            Result<DishDetailResult> result = new Result<DishDetailResult>();
            try
            {
                result = await SaveDetailAsync(request).ConfigureAwait(false);
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

        private async Task<Result<DishDetailResult>> SaveDetailAsync(DishDetailViewModelCreate request)
        {
            Result<DishDetailResult> result = new Result<DishDetailResult>();
            result.Data = new DishDetailResult(new Dictionary<string, IEnumerable<string>>());
            result.Data.Detail = new DishesDetailEntity();

            var detail = await DishDetailExists(request);

            if (detail != null)
            {
                result.Data.Detail = detail;
                result.StatusCode = "204";
                result.Message = "El ingrediente ya existe para este plato";
            }
            else
            {
                var data = await SaveDetailAsync(request, result.Data).ConfigureAwait(false);
                result.Data = data.dataResult;
                result.StatusCode = data.statusCode;
                result.Message = data.message;
            }

            return result;

        }

        private async Task<DishesDetailEntity> DishDetailExists(DishDetailViewModelCreate request)
        {
            return await dbContext.DishDetails.Where(m => m.DishId == request.DishId && m.IngredientId == request.IngredientId).FirstOrDefaultAsync();
        }

        private async Task<(DishDetailResult dataResult, string statusCode, string message)> SaveDetailAsync(DishDetailViewModelCreate request, DishDetailResult data)
        {
            (DishDetailResult dataResult, string statusCode, string message) result = (data, "", "");
            List<string> errors = await validateDetailAsync(request);
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
                    DishesDetailEntity dishDetail = new DishesDetailEntity()
                    {
                        DishId = request.DishId,
                        IngredientId = request.IngredientId,
                        Quantity = request.QuantityRequired
                    };
                    dbContext.AddRange(dishDetail);
                    await dbContext.SaveChangesAsync();

                    await tran.CommitAsync();
                    result.dataResult.Detail = dishDetail;
                    result.statusCode = "201";
                    result.message = "ingrediente insertado correctamente";
                }
                catch (Exception ex)
                {
                    await tran.RollbackAsync();
                    errors.Add($"Error al insertar ingrendiente: {ex.Message}");
                    result.dataResult.Errors.Add("1", errors);
                    result.statusCode = "400";
                }

            }

            return result;
        }

        private async Task<List<string>> validateDetailAsync(DishDetailViewModelCreate request)
        {
            List<string> errors = new List<string>();

            var isDuplicated = await dbContext.DishDetails.Where(m => m.DishId == request.DishId && m.IngredientId == request.IngredientId).AnyAsync();
            var dishExists = await dbContext.Dishes.Where(m => m.DishId == request.DishId).AnyAsync();
            var ingredientExits = await dbContext.Ingredients.Where(m => m.IngredientId == request.IngredientId).AnyAsync();

            if (isDuplicated)
            {
                errors.Add("El <b>Ingrediente</b> ya existe asociado para este <b>plato</b>");
            }
            if (!dishExists)
            {
                errors.Add("Este <b>plato</b> no existe");
            }
            if (!ingredientExits)
            {
                errors.Add("Este <b>ingrediente</b> no existe");
            }
            if (request.QuantityRequired <= 0)
            {
                errors.Add("La <b>Cantidad requerida</b> tiene que ser <b>mayor</b> que <b>cero</b>");
            }

            return errors;
        }
    }

}
