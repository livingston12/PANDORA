using System.Threading.Tasks;
using Pandora.Core.Models.Requests;
using Pandora.Core.Models.Responses;
using Pandora.Core.ViewModels;

namespace Pandora.Core.Interfaces
{
    public interface IMenuService : IPandoraService
    {
        Task<Response<MenuViewModel>> GetAsync(MenuRequest filter);
        Task<Response<CategoryViewModel>> GetCategoriesAsync(int menuId);
        Task<Response<DishViewModel>> GetDishesByCategoryAsync(int categoryId);
        Task<Response<DishViewModel>> GetDishesByMenusAsync(int menuId);
    }
}