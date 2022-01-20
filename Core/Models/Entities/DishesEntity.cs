using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pandora.Core.Attributes;

namespace Pandora.Core.Models.Entities
{
    [DefaultSortProperty("DishId")]
    [Table("Dishes")]
    public class DishesEntity : Entity
    {
        [Key]
        public int DishId { get; set; }
        [Required]
        public string Dish { get; set; }
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public CategoryEntity Category { get; set; }
        public IEnumerable<DishesDetailEntity> Ingredients { get; set; }

    }
}