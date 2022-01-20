using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pandora.Core.Attributes;

namespace Pandora.Core.Models.Entities
{
    [DefaultSortProperty("IngredientId")]
    [Table("Ingredients")]
    public class IngredientEntity : Entity
    {
        [Key]
        public int IngredientId { get; set; }
        [Required(ErrorMessage ="El ingrediente es obligatorio")]
        public string Ingredient { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        [Required(ErrorMessage ="El restaurante es obligatorio")]
        public int? RestaurantId { get; set; }
    }
}