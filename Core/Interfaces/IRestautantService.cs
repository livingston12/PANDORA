using System.Threading.Tasks;
using Pandora.Core.Models.Requests;
using Pandora.Core.Models.Responses;
using Pandora.Core.ViewModels;

namespace Pandora.Core.Interfaces
{
    public interface IRestaurantService : IPandoraService
    {
        Task<Response<RestaurantViewModel>> GetAsync(RestaurantRequest filter);
    }
}