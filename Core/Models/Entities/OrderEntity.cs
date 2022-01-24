using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pandora.Core.Attributes;

namespace Pandora.Core.Models.Entities
{
    [DefaultSortProperty("OrderId")]
    [Table("Orders")]
    public class OrdersEntity : Entity
    {
        [Key]
        public int OrderId { get; set; }
        [Required]
        public DateTime PlacementDate { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public int ItemsQuantity { get; set; }
        public decimal? Discount { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal? Subtotal { get; set; }
        public decimal? Tax { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal? Total { get; set; }
        public string Note { get; set; }
        public int? RestaurantId { get; set; }
        public IEnumerable<OrdersDetailEntity> Details { get; set; }

    }
}