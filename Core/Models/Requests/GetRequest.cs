using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Pandora.Core.Models.Requests
{
    public class GetRequest : Request
    {
        [NotMapped]
        public string Sort { get; set; }
        [NotMapped]
        [Range(1, int.MaxValue)]
        public int PageIndex { get; set; } = 1;
        [NotMapped]
        [Range(1, int.MaxValue)]
        public int PageSize { get; set; } = 10;
        [NotMapped]
        public string Message { get; set; }

        public virtual bool IsValidRequest()
        {
            if (PageIndex < 1)
            {
                Message = new StringBuilder("Page index should be greater than 0").ToString();
                return false;
            }

            if (PageSize < 1)
            {
                Message = new StringBuilder("Page size should be greater than 0").ToString();
                return false;
            }

            return true;
        }

        [NotMapped]
        public virtual int SkipSize => (PageIndex - 1) * PageSize;
    }
}