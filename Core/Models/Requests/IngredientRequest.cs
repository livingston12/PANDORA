using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pandora.Core.Models.Requests
{
    public class IngredientRequest : GetRequest
    {
        [NotMapped]
        public string IngredientIds { get; set; }
        public string Ingredient { get; set; }
        public decimal? Price { get; set; }
        public decimal? Quantity { get; set; }
        public int? RestaurantId { get; set; }
        
    }
}