using System.ComponentModel.DataAnnotations.Schema;

namespace Pandora.Core.Models.Requests
{
    public class RestaurantRequest : GetRequest
    {
        [NotMapped]
        public string RestaurantIds { get; set; }
        public string Name { get; set; }

    }
}