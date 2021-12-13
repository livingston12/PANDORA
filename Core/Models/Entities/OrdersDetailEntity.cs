using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pandora.Core.Attributes;

namespace Pandora.Core.Models.Entities
{
    [DefaultSortProperty("OrderDetailId")]
    [Table("OrdersDetail")]
    public class OrdersDetailEntity : Entity
    {
        [Key]
        public int OrderDetailId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal? Price { get; set; }
        public decimal? Tax { get; set; }
        public decimal? Discount { get; set; }
        public int? OrderId { get; set; }
        public int? DishId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public OrdersEntity Order { get; set; }
    }
}