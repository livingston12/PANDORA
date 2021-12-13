using System.Threading.Tasks;
using Pandora.Core.ViewModels;
using Pandora.Managers;

namespace Pandora.Core.Interfaces
{
    public interface IImageService
    {
        Task<(byte[] file, string extention)> GetAsync(string directory, string name);
        Task<ImageViewModel> PostAsync(FileUpload files, string directory);
    }
}