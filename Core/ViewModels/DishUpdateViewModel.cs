using System;

namespace Pandora.Core.ViewModels
{
    public class DishUpdateViewModel
    {
        public int DishId { get; set; }
        public string Dish { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? CategoryId { get; set; }
        public bool? NeedGarrison { get; set; } = false;
    }

}