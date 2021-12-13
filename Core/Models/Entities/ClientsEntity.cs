using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pandora.Core.Attributes;

namespace Pandora.Core.Models.Entities
{
    [DefaultSortProperty("ClientId")]
    [Table("Clients")]
    public class ClientsEntity : Entity
    {
        [Key]
        public int ClientId { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Name { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

    }
}