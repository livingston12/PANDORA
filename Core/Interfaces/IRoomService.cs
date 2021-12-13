using System.Threading.Tasks;
using Pandora.Core.Models.Requests;
using Pandora.Core.Models.Responses;
using Pandora.Core.ViewModels;

namespace Pandora.Core.Interfaces
{
    public interface IRoomService : IPandoraService
    {
        Task<Response<RoomViewModel>> GetAsync(RoomRequest filter);
        Task<Response<TableViewModel>> GetTablesAsync(int roomId);
    }
}