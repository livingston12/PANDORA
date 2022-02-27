using Pandora.Core.Models.Entities;

namespace Pandora.Core.ViewModels
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public int MenuId { get; set; }
        public MenusEntity Menu { get; set; }
    }
}