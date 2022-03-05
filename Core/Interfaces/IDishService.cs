using System.Collections.Generic;
using System.Threading.Tasks;
using Pandora.Core.Models;
using Pandora.Core.Models.Requests;
using Pandora.Core.Models.Responses;
using Pandora.Core.Models.Results;
using Pandora.Core.ViewModels;

namespace Pandora.Core.Interfaces
{
    public interface IDishService : IPandoraService
    {
        Task<Response<DishViewModel>> GetAsync(DishRequest filter);
        Task<Response<DishViewModel>> GetSummaryAsync(int? restaurantId);
        Task<IEnumerable<DishDetailViewModel>> GetDetailAsync(int dishId);
        Task<UpdateResult> PutDetailAsync(DishDetailRequest details);
        Task<UpdateResult> PutAsync(DishUpdateViewModel dish);
        Task<Result<DishResult>> CreateAsync(DishViewModelCreate request);
        Task<Result<DishDetailResult>> CreateDetailAsync(DishDetailViewModelCreate request);
        Task<UpdateResult> DeleteAsync(int dishId);
        Task<UpdateResult> DeleteDetailAsync(int dishDetailId);
    }
}