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
using Pandora.Core.Models.Dtos;
using Pandora.Core.Models.Entities;
using Pandora.Core.Models.Requests;
using Pandora.Core.Models.Responses;
using Pandora.Core.Models.Results;
using Pandora.Core.Utils;
using Pandora.Core.ViewModels;

namespace Pandora.Services
{
    public class OrderService : PandoraService, IOrderService
    {
        public OrderService(PandoraDbContext context)
           : base(context)
        {
        }

        public async Task<Result<OrderResult>> CreateRangeAsync(OrderCreateRequest request)
        {
            Check.NotNull(request, nameof(request));

            var result = new Result<OrderResult>();
            result = await CreateAsync(request);
            return result;
        }

        public async Task<Result<OrderResult>> CreateAsync(OrderCreateRequest request)
        {

            Result<OrderResult> result = new Result<OrderResult>();
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
        private async Task<Result<OrderResult>> SaveAsync(OrderCreateRequest request)
        {
            Check.NotNull(request, nameof(request));
            Check.NotNull(request.Invoice, nameof(request.Invoice));

            var result = new Result<OrderResult>();
            var tran = dbContext.Database.BeginTransaction();

            try
            {
                OrdersEntity orderEntity = OrderToEntity(request);
                IEnumerable<OrdersDetailEntity> ordersDetail = request.OrdersDetail
                .Select(m => new OrdersDetailEntity
                {
                    Discount = orderEntity.Discount,
                    DishId = m.DishId,
                    OrderId = orderEntity.OrderId,
                    Quantity = m.Quantity,
                    Note = m.Note,
                    Price = GetPriceOfItem(m.DishId)
                });
                result = await ChekOrderDetail(ordersDetail, request);


                if (!result.Data.Errors.Any())
                {
                    try
                    {
                        await dbContext.Orders.AddAsync(orderEntity).ConfigureAwait(false);
                        await dbContext.SaveChangesAsync();

                        result = await SaveDetailsAsync(ordersDetail).ConfigureAwait(false);
                        await dbContext.SaveChangesAsync();

                        result.Data.OrderId = orderEntity.OrderId.ToString();
                        result.Data.RestaurantId = orderEntity.RestaurantId;

                        result = await SaveInvoiceAsync(request.Invoice, result).ConfigureAwait(false);
                        await dbContext.SaveChangesAsync();

                        int isDiscountable = await DiscountInventoryAsync(orderEntity.ItemsQuantity, ordersDetail, request.Garrisons).ConfigureAwait(false);

                        result.StatusCode = "201";
                        result.Message = "Orden creada correctamente";
                        await tran.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await tran.RollbackAsync();
                        throw new Exception(ex.Message);
                    }
                }
                else
                {
                    result.Data.StatusCode = "400";
                }
            }
            catch (Exception ex)
            {
                result.StatusCode = "400";
                result.Data.Orders = null;
                result.Message = new StringBuilder($"<b>No se pudo crear la orden:</b> {ex.Message}").ToString();
            }
            finally
            {
                await tran.DisposeAsync();
            }

            return result;
        }

        private OrdersEntity OrderToEntity(OrderCreateRequest request)
        {
            OrdersEntity result = new OrdersEntity()
            {
                Discount = request.Discount,
                ItemsQuantity = request.OrdersDetail.Count(),
                Note = request.Note,
                PlacementDate = DateTime.Now,
                Status = request.Status,
                Tax = request.Tax,
                RestaurantId = request.RestaurantId
            };
            return result;
        }

        private decimal? GetPriceOfItem(int dishId)
        {
            var currenDish = dbContext.Dishes.Where(x => x.DishId == dishId).FirstOrDefault();
            return currenDish.Price;
        }

        private async Task<Result<OrderResult>> ChekOrderDetail(IEnumerable<OrdersDetailEntity> ordersDetail, OrderCreateRequest request)
        {
            int restaurantId = request.RestaurantId;
            var result = await validateInvoice(request.Invoice);

            if (!result.Data.Errors.Any())
            {
                result = new Result<OrderResult>();
                result.Data = new OrderResult(new Dictionary<string, IEnumerable<string>>());
                var messages = new List<(string Index, IEnumerable<string> Text)>();
                int index = 0;

                foreach (var item in ordersDetail)
                {
                    index++;
                    var currenDish = await dbContext.Dishes.Where(x => x.DishId == item.DishId).FirstOrDefaultAsync();
                    var error = await validateOrderItem(index, item, currenDish, request.Garrisons, restaurantId);

                    if (error.Text.Any())
                    {
                        messages.Add(error);
                        continue;
                    }
                }

                var listErrors = errorMessages(messages);
                foreach (var error in listErrors)
                {
                    result.Data.Errors.Add(error);
                }
            }

            return result;
        }

        private async Task<Result<OrderResult>> validateInvoice(InvoicesRequest invoice)
        {
            var result = new Result<OrderResult>();
            List<string> errors = new List<string>();
            List<string> paymentAvalible = new List<string>
            (
                new List<string>() { "E", "T" }
            );
            result.Data = new OrderResult(new Dictionary<string, IEnumerable<string>>());

            var currentTable = await dbContext.Tables.Where(m => m.TableId == invoice.TableId).FirstOrDefaultAsync();

            if (currentTable == null)
            {
                errors.Add("La mesa no existe");
            }
            if (!paymentAvalible.Contains(invoice.PaymentMethod))
            {
                errors.Add("Los metodos de pagos dispoibles son <b>Efectivo</b> o <b>Tarjeta</b>");
            }
            if (errors.Any())
            {
                result.Data.Errors.Add("1", errors);
            }

            return result;
        }

        private async Task<(string Index, IEnumerable<string> Text)> validateOrderItem(int index, OrdersDetailEntity item, DishesEntity currenDish, IEnumerable<GarrisonsRequest> Garrisons, int restaurantId)
        {
            (string Index, IEnumerable<string> Text) result = ("", new List<string>());
            List<string> errors = new List<string>();
            if (currenDish == null)
            {
                errors.Add($"El plato introducido no existe");
            }
            else
            {
                var category = await dbContext.Categories.Where(x => x.CategoryId == currenDish.CategoryId).FirstOrDefaultAsync();
                var restaurant = await dbContext.Menus.Where(x => x.MenuId == category.MenuId && x.RestaurantId == restaurantId).FirstOrDefaultAsync();

                if (restaurant == null)
                {
                    errors.Add($"<b>{currenDish.Dish}:</b> no pertenece a este restaurante");
                }
                if (item.Quantity < 1)
                {
                    errors.Add($"<b>{currenDish.Dish}:</b> no puede tener La cantidad mayor que cero");
                }
                if (item.Price < 1)
                {
                    errors.Add($"<b>{currenDish.Dish}:</b> no puede tener el precio mayor que cero");
                }
                if (Garrisons == null && currenDish.NeedGarrison == true)
                {
                    errors.Add($"El plato <b>{currenDish.Dish}</b> requiere Guarnicion");
                }
                else if (currenDish.NeedGarrison == true)
                {
                    List<string> garrisonErrors = await validateGarrisons(currenDish.DishId, currenDish.Dish, Garrisons);

                    if (garrisonErrors.Any())
                    {
                        errors.AddRange(garrisonErrors);
                    }
                }
            }
            if (errors.Any())
            {
                result.Index = index.ToString();
                result.Text = errors;
            }

            return result;
        }
        private async Task<List<string>> validateGarrisons(int? dishId, string dish, IEnumerable<GarrisonsRequest> garrisons)
        {
            List<string> errors = new List<string>();

            var listGarrisons = garrisons.Where(m => m.DishId == dishId);
            var ingredientIds = listGarrisons.Select(m => m.IngredientId);

            if (!listGarrisons.Any())
            {
                errors.Add($"El plato <b>{dish}:</b> requiere Guarnicion");
            }
            else if (listGarrisons.Any())
            {
                var notQuantities = listGarrisons.FirstOrDefault(m => m.Quantity <= 0);
                if (notQuantities != null)
                {
                    errors.Add($"El plato <b>{dish}:</b> requiere cantidad en las guarniciones");
                }
            }
            else
            {
                var ingredientExist = await dbContext.Ingredients.Where(m => ingredientIds.Contains(m.IngredientId)).AnyAsync();

                if (!ingredientExist)
                {
                    errors.Add($"Los <b>ingredientes de la guarnicion</b> deben existir en el plato (<b>{dish}:</b>)");
                }
            }

            return errors;
        }
        private List<KeyValuePair<string, IEnumerable<string>>> errorMessages(List<(string Index, IEnumerable<string> Text)> messages)
        {
            return messages
               .AsEnumerable()
               .Select(item =>
                  new KeyValuePair<string, IEnumerable<string>>(item.Index, item.Text)
               )
           .ToList();
        }

        private async Task<Result<OrderResult>> SaveDetailsAsync(IEnumerable<OrdersDetailEntity> ordersDetail)
        {
            var result = new Result<OrderResult>();
            var listRequestDto = new List<OrderRequestDto>();
            result.Data = new OrderResult(new Dictionary<string, IEnumerable<string>>());
            result.Data.Orders = listRequestDto;

            await dbContext.OrdersDetail.AddRangeAsync(ordersDetail).ConfigureAwait(false);
            listRequestDto = ordersDetail
            .Select(x => new OrderRequestDto()
            {
                DishId = x.DishId,
                Note = string.Empty,
                PlacementDate = DateTime.Now
            })
            .ToList();
            result.Data.Orders = listRequestDto;

            return result;
        }

        private async Task<Result<OrderResult>> SaveInvoiceAsync(InvoicesRequest invoice, Result<OrderResult> result)
        {
            if (int.TryParse(result.Data.OrderId, out int orderId))
            {
                InvoiceEntity invoiceEntity = new InvoiceEntity()
                {
                    ClientId = invoice.ClientId,
                    OrderId = orderId,
                    PaymentMethod = invoice.PaymentMethod,
                    TableId = invoice.TableId,
                    UserId = invoice.UserId
                };
                await dbContext.Invoices.AddAsync(invoiceEntity).ConfigureAwait(false);
            }
            else
            {
                throw new Exception("La orden no existe");
            }

            return result;
        }

        private async Task<int> DiscountInventoryAsync(int itemsQuantity, IEnumerable<OrdersDetailEntity> ordersDetail, IEnumerable<GarrisonsRequest> garrisons)
        {
            int result = 0;

            try
            {
                foreach (var di in ordersDetail)
                {
                    // Update current dish Quantity 
                    int? dishId = di.DishId;
                    var dish = await dbContext.Dishes
                                                .FirstOrDefaultAsync(m => m.DishId == dishId);
                    

                    var dishesDetail = await dbContext.DishDetails.Where(m => m.DishId == dishId).ToListAsync();
                    if (!dishesDetail.Any() && garrisons.Any())
                    {
                        var details = garrisons
                            .Select(m => new DishesDetailEntity()
                            {
                                DishId = m.DishId.Value
                            })
                            .ToList();
                        dishesDetail.AddRange(details);
                    }
                    else if (!dishesDetail.Any())
                    {
                        if (checkMaximumNumber(dish.Quantity, di.Quantity))
                        {
                            throw new Exception($"El plato solo tiene <b>({dish.Quantity})</b> en inventario y esta comprando <b>({itemsQuantity})</b>");
                        }

                        dish.Quantity -= di.Quantity;
                        dbContext.Entry(dish);
                    }
                    // Update ingredients quantity from dish detail 
                    foreach (var dishDetail in dishesDetail)
                    {
                        IngredientEntity ingredient;
                        int? quantity = 0;

                        if (dish.NeedGarrison == true)
                        {
                            var ingredientIds = garrisons.Where(m => m.DishId == dish.DishId).Select(m => m.IngredientId);
                            var ingredients = dbContext.Ingredients.Where(m => ingredientIds.Contains(m.IngredientId));

                            foreach (var g in garrisons)
                            {
                                var i = ingredients.FirstOrDefault(m => m.IngredientId == g.IngredientId && m.IsGarrison == true);

                                if (i == null || checkMaximumNumber(i.Quantity, quantity))
                                {
                                    throw new Exception($"El ingrediente ({i.Ingredient}) del plato ({dish.Dish}) no tiene la disponibilidad requerida");
                                }
                                i.Quantity -= di.Quantity * g.Quantity;
                                dbContext.Entry(i);
                            }
                        }
                        else
                        {
                            ingredient = await dbContext.Ingredients.FirstOrDefaultAsync(m => m.IngredientId == dishDetail.IngredientId);
                            quantity = dishDetail.Quantity;
                            if (checkMaximumNumber(ingredient.Quantity, quantity))
                            {
                                throw new Exception($"El ingrediente ({ingredient.Ingredient}) del plato ({dish.Dish}) no tiene la disponibilidad requerida");
                            }

                            ingredient.Quantity -= quantity;

                            dbContext.Entry(ingredient);
                        }


                    }

                    result = await dbContext.SaveChangesAsync();
                }
            }
            catch (System.Exception ex)
            {
                throw new Exception($"Error inesperado al actualizar inventario: {ex.Message}");
            }

            return result;
        }

        private static bool checkMaximumNumber(int? maximun, int? minimun)
        {
            return maximun < minimun;
        }

        public async Task<Response<OrderViewModel>> GetAsync(OrderRequest filter)
        {
            Check.NotNull(filter, nameof(filter));

            Response<OrderViewModel> result = null;
            var query = GetQuery(filter);
            var total = await query.CountAsync().ConfigureAwait(false);
            if (total > 0)
            {
                var queryPaging = MapToViewModel(query)
                        .Skip(filter.SkipSize)
                        .Take(filter.PageSize);
                result = new Response<OrderViewModel>()
                {
                    List = queryPaging,
                    PageIndex = filter.PageIndex,
                    PageSize = filter.PageSize,
                    Total = total
                };
            }

            return result;
        }

        private IQueryable<OrdersEntity> GetQuery(OrderRequest filter)
        {
            var query = dbContext
                            .Orders
                            .Include(x => x.Details)
                            .AsQueryable();

            query = ApplyFilters(query, filter);
            query = query.OrderBy(filter.Sort);

            return query;
        }

        private static IQueryable<OrdersEntity> ApplyFilters(IQueryable<OrdersEntity> query, OrderRequest filter)
        {
            IQueryable<OrdersEntity> filteredQuery = query;
            filteredQuery = filteredQuery.Where(filter);
            filteredQuery = ApplyOrderIdsFilters(filteredQuery, filter.OrderIds);
            return filteredQuery;
        }

        private static IQueryable<OrdersEntity> ApplyOrderIdsFilters(IQueryable<OrdersEntity> query, string OrderIds)
        {
            if (OrderIds.IsNotNullOrEmpty())
            {
                query = query.Where(p => OrderIds.Contains(p.OrderId.ToString()));
            }

            return query;
        }

        private static IEnumerable<OrderViewModel> MapToViewModel(IEnumerable<OrdersEntity> query)
        {
            return query
                .Select(m => new OrderViewModel
                {
                    OrderId = m.OrderId,
                    PlacementDate = m.PlacementDate,
                    Discount = m.Discount,
                    ItemsQuantity = m.ItemsQuantity,
                    Status = m.Status,
                    Tax = m.Tax,
                    Note = m.Note,
                    SubTotal = m.Subtotal,
                    Total = m.Total,
                    RestaurantId = m.RestaurantId,
                    Details = m.Details.Select(x => new OrdersDetailViewModel()
                    {
                        OrderDetailId = x.OrderDetailId,
                        DishId = x.DishId,
                        Price = x.Price,
                        Quantity = x.Quantity,
                        Total = x.Price * x.Quantity
                    })
                });
        }

        public async Task<OrderTotalViewModel> GetTotalByTablesAsync(OrderTotalRequest request)
        {
            List<(decimal? total, string label)> lisTotals = new List<(decimal? total, string label)>();

            OrderTotalViewModel result = new OrderTotalViewModel();
            var orders = await dbContext.Orders.Where(m => m.RestaurantId == request.RestaurantId).ToListAsync();
            orders = checkDateFilters(orders, request.DateFrom, request.DateTo);

            if (orders.Any())
            {
                foreach (var order in orders)
                {
                    var invoce = await dbContext.Invoices.Where(m => m.OrderId == order.OrderId).FirstOrDefaultAsync();
                    if (invoce == null)
                    {
                        continue;
                    }

                    string TableName = await GetCurrentTableName(invoce.TableId);
                    if (!string.IsNullOrEmpty(TableName))
                    {
                        lisTotals.Add((order.Total, TableName));
                    }
                }
            }

            if (lisTotals.Any())
            {
                result = TotalToOrderTotalViewModel(lisTotals);

            }

            return result;
        }

        private static OrderTotalViewModel TotalToOrderTotalViewModel(List<(decimal? total, string label)> lisTotals)
        {
            return new OrderTotalViewModel()
            {
                Labels = lisTotals
                                .GroupBy(m => m.label)
                                .Select(m => m.Key)
                                .ToList(),
                Totals = lisTotals
                                .GroupBy(m => m.label)
                                .Select(m =>
                                            m.Where(x => x.label == m.Key)
                                            .Sum(m => m.total)
                                ).ToList()
            };
        }

        private static List<OrdersEntity> checkDateFilters(List<OrdersEntity> orders, DateTime? dateFrom, DateTime? dateTo)
        {
            if (!dateFrom.HasValue)
            {
                orders = orders.Where(m => m.PlacementDate.Date >= DateTime.Today).ToList();
            }
            else if (!dateTo.HasValue)
            {
                orders = orders.Where(m =>
                        m.PlacementDate.Date >= dateFrom.Value.Date &&
                        m.PlacementDate.Date <= DateTime.Today
                    ).ToList();
            }
            else
            {
                orders = orders.Where(m =>
                        m.PlacementDate.Date >= dateFrom.Value.Date &&
                        m.PlacementDate.Date <= dateTo.Value.Date
                    ).ToList();
            }
            return orders;
        }

        private async Task<string> GetCurrentTableName(int tableId)
        {
            string result = string.Empty;
            var currentTable = await dbContext.Tables.FirstOrDefaultAsync(m => m.TableId == tableId);

            if (currentTable != null)
            {
                result = currentTable.Table;
            }
            return result;
        }
        public async Task<OrderTotalViewModel> GetTotalByDeliveryAsync(OrderTotalRequest request)
        {
            List<(decimal? total, string label)> lisTotals = new List<(decimal? total, string label)>();

            OrderTotalViewModel result = new OrderTotalViewModel();
            var orders = await dbContext.Orders.Where(m => m.RestaurantId == request.RestaurantId).ToListAsync();
            orders = checkDateFilters(orders, request.DateFrom, request.DateTo);

            if (orders.Any())
            {
                foreach (var order in orders)
                {
                    var invoce = await dbContext.Invoices.Where(m => m.OrderId == order.OrderId).FirstOrDefaultAsync();
                    if (invoce == null)
                    {
                        continue;
                    }

                    string ClientName = await GetCurrentClient(invoce.ClientId);
                    if (!string.IsNullOrEmpty(ClientName))
                    {
                        lisTotals.Add((order.Total, ClientName));
                    }
                    else
                    {
                        lisTotals.Add((order.Total, "Sin delivery"));
                    }
                }
            }

            if (lisTotals.Any())
            {
                result = TotalToOrderTotalViewModel(lisTotals);

            }

            return result;
        }

        private async Task<string> GetCurrentClient(int? clientId)
        {
            string result = string.Empty;
            var currentClient = await dbContext.Clients.FirstOrDefaultAsync(m => m.ClientId == clientId);

            if (currentClient != null)
            {
                result = $"{currentClient.Name} {currentClient.LastName}";
            }
            return result;
        }
    }

}
