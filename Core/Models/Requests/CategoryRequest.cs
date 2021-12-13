using System.ComponentModel.DataAnnotations.Schema;

namespace Pandora.Core.Models.Requests
{
    public class CategoryRequest : GetRequest
    {
        [NotMapped]
        public string CategoryIds { get; set; }
        public string Category { get; set; }
        [NotMapped]
        public string MenuIds { get; set; }
    }
}