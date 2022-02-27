using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pandora.Core.Models.Requests
{
    public class OrderCreateRequest
    {
        [RegularExpression(@"Canceled|Active|Close|Delivered", ErrorMessage = "Los status permitido son (Canceled|Active|Close|Delivered)")]
        public string Status { get; set; }
        public decimal? Discount { get; set; }
        public decimal? Tax { get; set; }
        public string Note { get; set; }
        [Required(ErrorMessage = "El restaurante es obligatorio")]
        public int RestaurantId { get; set; }
        public IEnumerable<OrderDetailRequest> OrdersDetail { get; set; }
        public InvoicesRequest Invoice { get; set; }
        public IEnumerable<GarrisonsRequest> Garrisons { get; set; }
    }

    public class OrderDetailRequest
    {
        public int Quantity { get; set; }
        public int DishId { get; set; }
        public string Note { get; set; }
    }
}