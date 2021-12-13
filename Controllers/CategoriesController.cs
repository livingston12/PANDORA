
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pandora.Core.Attributes;
using Pandora.Core.Interfaces;
using Pandora.Core.Models.Requests;
using Pandora.Core.Models.Responses;
using Pandora.Core.ViewModels;
using System.Threading.Tasks;

namespace Pandora.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiVersion("1.0")]
    [ApiExceptionFilter]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoriesService CategoryService;
        
        public CategoriesController(
            ICategoriesService CategoryService
            )
        {
            this.CategoryService = CategoryService;
        }

        [HttpGet]
        //[JwtAuthorize("Hydra.Accounts.Read")]
        [ProducesResponseType(typeof(Response<CategoryViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<Response<CategoryViewModel>> GetAsync([FromQuery] CategoryRequest request)
        {
            return await CategoryService.GetAsync(request).ConfigureAwait(false);
        }
        
    }
}