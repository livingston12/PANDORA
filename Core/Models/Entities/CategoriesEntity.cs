using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pandora.Core.Attributes;

namespace Pandora.Core.Models.Entities
{
    [DefaultSortProperty("CategoryId")]
    [Table("Categories")]
    public class CategoryEntity : Entity
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        public string Category { get; set; }
        public int MenuId { get; set; }
    }
}