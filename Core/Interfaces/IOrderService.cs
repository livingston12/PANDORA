using System.Collections.Generic;
using System.Threading.Tasks;
using Pandora.Core.Models;
using Pandora.Core.Models.Requests;
using Pandora.Core.Models.Responses;
using Pandora.Core.ViewModels;

namespace Pandora.Core.Interfaces
{
    public interface IOrderService : IPandoraService
    {
        Task<Response<OrderViewModel>> GetAsync(OrderRequest filter);
        Task<Result<OrderResult>> CreateRangeAsync(OrderCreateRequest request);
        
    }
}