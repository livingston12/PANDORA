using Pandora.Core.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pandora.Core.Models.Entities
{
    [DefaultSortProperty("RestaurantId")]
    [Table("Restaurants")]
    public class RestaurantsEntity : Entity
    {
        [Key]
        public int RestaurantId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Document { get; set; }
        public byte? Image { get; set; }
    }

}