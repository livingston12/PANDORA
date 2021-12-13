using Microsoft.AspNetCore.Http;

namespace Pandora.Managers
{
    public class FileUpload
    {
       public enum TypeImage
        {
            inserted,
            error,
            empty
        }
        public IFormFile files { get; set; }
    }
}