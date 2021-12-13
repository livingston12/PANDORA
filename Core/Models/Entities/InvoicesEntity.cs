using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pandora.Core.Attributes;

namespace Pandora.Core.Models.Entities
{
    [DefaultSortProperty("InvoiceId")]
    [Table("Invoices")]
    public class InvoicesEntity
    {
        [Key]
        public int InvoiceId { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int OrderId { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int TableId { get; set; }
        public int? ClientId { get; set; }
        public int? UserId { get; set; }
        public string PaymentMethod { get; set; }


    }
}