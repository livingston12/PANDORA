using System.Threading.Tasks;
using Pandora.Core.Models;
using Pandora.Core.Models.Requests;
using Pandora.Core.Models.Responses;
using Pandora.Core.Models.Results;
using Pandora.Core.ViewModels;

namespace Pandora.Core.Interfaces
{
    public interface ICategoriesService : IPandoraService
    {
        Task<Response<CategoryViewModel>> GetAsync(CategoryRequest filter);
        Task<Response<CategoryViewModel>> GetSummaryAsync();
        Task<Result<CategoryResult>> CreateAsync(CategoryCreateRequest request);
    }
}