using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pandora.Core.Models.Requests
{
    public class RoomRequest : GetRequest
    {
        [NotMapped]
        public string RoomIds { get; set; }
        public string Room { get; set; }
        public string Description { get; set; }
        
    }
}