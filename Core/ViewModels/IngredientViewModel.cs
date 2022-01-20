using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pandora.Core.ViewModels
{
    public class IngredientViewModel
    {
        public int IngredientId { get; set; }
        public string Ingredient { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public int? RestaurantId { get; set; }
    }
}