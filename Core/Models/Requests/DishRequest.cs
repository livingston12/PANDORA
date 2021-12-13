using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pandora.Core.Models.Requests
{
    public class DishRequest : GetRequest
    {
        [NotMapped]
        public string DishIds { get; set; }
        public string Dish { get; set; }
        public string Description { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? CategoryId { get; set; }
        
    }
}