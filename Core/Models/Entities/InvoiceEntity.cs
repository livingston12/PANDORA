using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pandora.Core.Attributes;

namespace Pandora.Core.Models.Entities
{
    [DefaultSortProperty("InvoiceId")]
    [Table("Invoices")]
    public class InvoiceEntity : Entity
    {
        [Key]
        public int InvoiceId { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int OrderId { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int TableId { get; set; }
        public int? ClientId { get; set; }
        public int? UserId { get; set; }
        [RegularExpression(@"E|T|C", ErrorMessage = "Los status permitido son (E(Efectivo), T(Tarjeta) or C(Cheque))")]
        public string PaymentMethod { get; set; }
        public OrdersEntity Order { get; set; }
        public TablesEntity Table { get; set; }

    }
}