using System.Collections.Generic;
using Pandora.Core.Models.Entities;

namespace Pandora.Core.ViewModels
{
    public class RoomViewModel
    {
        public int RoomId { get; set; }
        public string Room { get; set; }
        public string Description { get; set; }
        public IEnumerable<TablesEntity> Tables {get;set;}
        
    }
}