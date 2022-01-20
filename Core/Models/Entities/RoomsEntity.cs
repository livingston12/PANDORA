using Pandora.Core.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pandora.Core.Models.Entities
{
    [DefaultSortProperty("RoomId")]
    [Table("Rooms")]
    public class RoomsEntity : Entity
    {
        [Key]
        public int RoomId { get; set; }
        [Required]
        public string Room { get; set; }
        public string Description { get; set; }
        public IEnumerable<TablesEntity> Tables { get; set; }
    }

}