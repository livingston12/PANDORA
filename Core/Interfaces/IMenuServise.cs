using System.Collections.Generic;
using System.Threading.Tasks;
using Pandora.Core.Models.Requests;
using Pandora.Core.Models.Responses;
using Pandora.Core.ViewModels;

namespace Pandora.Core.Interfaces
{
    public interface IMenuService : IPandoraService
    {
        Task<Response<MenuViewModel>> GetAsync(MenuRequest filter);
        Task<Response<CategoryViewModel>> GetCategories(int menuId);
        Task<Response<DishViewModel>> GetDishes(int categoryId);
    }
}