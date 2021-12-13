using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Pandora.Core.Models.Entities;

namespace Pandora.Core.Models.Requests
{
    public class InvoicesRequest 
    {
        public int TableId { get; set; }
        public int? ClientId { get; set; }
        public int? UserId { get; set; }
        public string PaymentMethod { get; set; }
    }
}