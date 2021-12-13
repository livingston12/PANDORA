using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pandora.Core.Models.Requests
{
    public class MenuRequest : GetRequest
    {
        [NotMapped]
        public string MenuIds { get; set; }
        public string Menu { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int RestaurantId { get; set; }
        
    }
}