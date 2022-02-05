using System.ComponentModel.DataAnnotations.Schema;
using Pandora.Core.Models.Entities;

namespace Pandora.Core.ViewModels
{
    public class InvoiceViewModel
    {
        public int InvoiceId { get; set; }
        public int OrderId { get; set; }
        public int TableId { get; set; }
        public int? ClientId { get; set; }
        public int? UserId { get; set; }
        public string PaymentMethod { get; set; }
        public OrdersEntity Order { get; set; }
        [ForeignKey(nameof(TableId))]
        public TablesEntity Table { get; set; }
    }
}