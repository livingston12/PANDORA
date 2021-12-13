using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pandora.Core.Models.Requests
{
    public class OrderRequest : GetRequest
    {
        [NotMapped]
        public string OrderIds { get; set; }
        public DateTime? PlacementDate { get; set; }
        public string Status { get; set; }
        public int RestaurantId { get; set; }
    }
}