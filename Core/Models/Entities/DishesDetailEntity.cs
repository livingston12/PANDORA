using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pandora.Core.Attributes;

namespace Pandora.Core.Models.Entities
{
    [DefaultSortProperty("DishId")]
    [Table("DishesDetail")]
    public class DishesDetailEntity : Entity
    {
        [Key]
        public int DishDetailId { get; set; }
        [Required]
        public int DishId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public int IngredientId { get; set; }
        [ForeignKey(nameof(DishId))]
        public DishesEntity Dish { get; set; }
        [ForeignKey(nameof(IngredientId))]
        public IngredientEntity Ingredient { get; set; }
    }
}