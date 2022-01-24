using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pandora.Core.Models.Requests
{
    public class OrderTotalRequest
    {
        [NotMapped]
        public DateTime? DateFrom { get; set; }
        [NotMapped]
        public DateTime? DateTo { get; set; }
        [NotMapped]
        public int? RestaurantId { get; set; }
    }
}