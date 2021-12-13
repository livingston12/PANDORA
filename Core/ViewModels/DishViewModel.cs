using System;
using System.Collections.Generic;

namespace Pandora.Core.ViewModels
{
    public class DishViewModel
    {
        public int DishId { get; set; }
        public string Dish { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public byte? Image { get; set; }
        public int? CategoryId { get; set; }
        public IEnumerable<DishesDetailViewModel> Ingredients { get; set; }
    }

    public class DishesDetailViewModel
    {
        public int DishDetailId { get; set; }
        public int DishId { get; set; }
        public int IngredientId { get; set; }
    }
}