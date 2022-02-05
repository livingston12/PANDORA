using System.Threading.Tasks;
using Pandora.Core.Models.Requests;
using Pandora.Core.Models.Responses;
using Pandora.Core.ViewModels;

namespace Pandora.Core.Interfaces
{
    public interface ITableService : IPandoraService
    {
        Task<Response<TableViewModel>> GetAsync(TableRequest filter);
        Task<bool> ReservedAsync(TableReservedRequest tableId);
    }
}