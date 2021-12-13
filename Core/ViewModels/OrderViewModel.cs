using System;
using System.Collections.Generic;

namespace Pandora.Core.ViewModels
{
    public class OrderViewModel 
    {
        public int OrderId { get; set; }
        public DateTime PlacementDate { get; set; }
        public string Status { get; set; }
        public int ItemsQuantity { get; set; }
        public decimal? Discount { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? Tax { get; set; }
        public decimal? Total { get; set; }
        public string Note { get; set; }
        public int? RestaurantId { get; set; }
        public IEnumerable<OrdersDetailViewModel> Details { get; set; }
    }
}