using Pandora.Core.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pandora.Core.ViewModels
{
    [DefaultSortProperty("TableId")]
    [Table("Tables")]
    public class TableViewModel
    {
        [Key]
        public int TableId { get; set; }
        [Required]
        public string Table { get; set; }
        public string Description { get; set; }
        [Required]
        public bool Active { get; set; }
        public int RoomId { get; set; }
        public int RestaurantId { get; set; }
        public bool? IsReserved { get; set; }
    }

}