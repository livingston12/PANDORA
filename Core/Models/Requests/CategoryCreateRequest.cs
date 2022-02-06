namespace Pandora.Core.Models.Requests
{
    public class CategoryCreateRequest
    {
        public string Category { get; set; }
        public int? MenuId { get; set; }
    }
}