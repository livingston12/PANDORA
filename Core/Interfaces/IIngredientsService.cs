using System.Collections.Generic;
using System.Threading.Tasks;
using Pandora.Core.Models;
using Pandora.Core.Models.Requests;
using Pandora.Core.Models.Responses;
using Pandora.Core.Models.Results;
using Pandora.Core.ViewModels;

namespace Pandora.Core.Interfaces
{
    public interface IIngredientService : IPandoraService
    {
        Task<Response<IngredientViewModel>> GetAsync(IngredientRequest request);
        Task<IEnumerable<IngredientViewModel>> GetSummaryAsync(int restaurantId);
        Task<IEnumerable<IngredientViewModel>> GetGarrisonsAsync(int restaurantId);
        Task<Result<IngredientResult>> CreateAsync(IngredientViewModel request);
        Task<UpdateResult> PutAsync(IngredientViewModel ingredient);
    }
}