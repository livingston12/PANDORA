using System;
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
    public class InvoceService : PandoraService, IInvoiceService
    {
        public InvoceService(PandoraDbContext context)
           : base(context)
        {
        }

        public async Task<Response<InvoiceViewModel>> GetAsync(InvoiceRequest filter)
        {
            Check.NotNull(filter, nameof(filter));

            Response<InvoiceViewModel> result = null;
            var query = GetQuery(filter);
            var total = await query.CountAsync().ConfigureAwait(false);
            if (total > 0)
            {
                var queryPaging = MapToViewModel(query)
                        .Skip(filter.SkipSize)
                        .Take(filter.PageSize);
                result = new Response<InvoiceViewModel>()
                {
                    List = queryPaging,
                    PageIndex = filter.PageIndex,
                    PageSize = filter.PageSize,
                    Total = total
                };
            }

            return result;
        }

        private IQueryable<InvoiceEntity> GetQuery(InvoiceRequest filter)
        {
            var query = dbContext
                            .Invoices
                            .Include(x => x.Order)
                            .Include(x => x.Table)
                            .AsQueryable();

            query = ApplyFiltersFromDates(query, filter.DateFrom, filter.DateTo);
            query = ApplyFilters(query, filter);
            query = query.OrderBy(filter.Sort);

            return query;
        }

        private static IQueryable<InvoiceEntity> ApplyFiltersFromDates(IQueryable<InvoiceEntity> query, DateTime? dateFrom, DateTime? dateTo)
        {
            Check.NotNull(query, nameof(query));
            IQueryable<InvoiceEntity> filteredQuery = query;
            if (dateFrom.HasValue)
            {
                filteredQuery = filteredQuery.Where(m => m.Order.PlacementDate.Date >= dateFrom.Value.Date);
            }
            if (dateTo.HasValue)
            {
                filteredQuery = filteredQuery.Where(m => m.Order.PlacementDate.Date <= dateTo.Value.Date);
            }
            return filteredQuery;
        }

        private static IQueryable<InvoiceEntity> ApplyFilters(IQueryable<InvoiceEntity> query, InvoiceRequest filter)
        {
            IQueryable<InvoiceEntity> filteredQuery = query;
            filteredQuery = filteredQuery.Where(filter);
            filteredQuery = ApplyFilters(filteredQuery, filter.InvoiceIds, filter.OrderIds, filter.TableIds, filter.ClientIds, filter.UserIds);
            return filteredQuery;
        }

        private static IQueryable<InvoiceEntity> ApplyFilters(IQueryable<InvoiceEntity> query, string InvoiceIds, string OrderIds, string TableIds, string ClientIds, string UserIds)
        {
            if (InvoiceIds.IsNotNullOrEmpty())
            {
                query = query.Where(p => InvoiceIds.Contains(p.InvoiceId.ToString()));
            }
            if (OrderIds.IsNotNullOrEmpty())
            {
                query = query.Where(p => OrderIds.Contains(p.OrderId.ToString()));
            }
            if (TableIds.IsNotNullOrEmpty())
            {
                query = query.Where(p => TableIds.Contains(p.TableId.ToString()));
            }
            if (ClientIds.IsNotNullOrEmpty())
            {
                query = query.Where(p => ClientIds.Contains(p.ClientId.ToString()));
            }
            if (UserIds.IsNotNullOrEmpty())
            {
                query = query.Where(p => UserIds.Contains(p.UserId.ToString()));
            }
            return query;
        }

        private static IEnumerable<InvoiceViewModel> MapToViewModel(IEnumerable<InvoiceEntity> query)
        {
            return query
                .Select(m => new InvoiceViewModel
                {
                    InvoiceId = m.InvoiceId,
                    OrderId = m.OrderId,
                    TableId = m.TableId,
                    ClientId = m.ClientId,
                    UserId = m.UserId,
                    PaymentMethod = m.PaymentMethod,
                    Order = m.Order,
                    Table = m.Table
                });
        }

    }
}