using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pandora.Core.Models.Requests
{
    public class InvoiceRequest : GetRequest
    {
        [NotMapped]
        public string InvoiceIds { get; set; }
        [NotMapped]
        public string OrderIds { get; set; }
        [NotMapped]
        public string TableIds { get; set; }
        [NotMapped]
        public string ClientIds { get; set; }
        [NotMapped]
        public string UserIds { get; set; }
        [NotMapped]
        public DateTime? DateFrom { get; set; }
        [NotMapped]
        public DateTime? DateTo { get; set; }

        [RegularExpression(@"E|T|C|''", ErrorMessage = "Los status permitido son (E(Efectivo), T(Tarjeta) or C(Cheque))")]
        public string PaymentMethod { get; set; }
        public int? RestaurantId { get; set; }
    }
}