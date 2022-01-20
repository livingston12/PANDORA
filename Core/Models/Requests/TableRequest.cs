using System.ComponentModel.DataAnnotations.Schema;

namespace Pandora.Core.Models.Requests
{
    public class TableRequest : GetRequest
    {
        [NotMapped]
        public string TableIds { get; set; }
        public string Table { get; set; }
        public bool? Active { get; set; }
        [NotMapped]
        public string RoomId { get; set; }

    }
}