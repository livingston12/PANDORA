using System.Threading.Tasks;
using Pandora.Core.Models.Requests;
using Pandora.Core.Models.Responses;
using Pandora.Core.ViewModels;

namespace Pandora.Core.Interfaces
{
    public interface IDishService : IPandoraService
    {
        Task<Response<DishViewModel>> GetAsync(DishRequest filter);
    }
}