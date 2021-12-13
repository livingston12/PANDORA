using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pandora.Core.Attributes;

namespace Pandora.Core.Models.Entities
{
    [DefaultSortProperty("MenuId")]
    [Table("Menus")]
    public class MenusEntity : Entity
    {
        [Key]
        public int MenuId { get; set; }
        [Required]
        public string Menu { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? RestaurantId { get; set; }


    }
}