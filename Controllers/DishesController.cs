
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pandora.Core.Attributes;
using Pandora.Core.Interfaces;
using Pandora.Core.Models;
using Pandora.Core.Models.Requests;
using Pandora.Core.Models.Responses;
using Pandora.Core.Models.Results;
using Pandora.Core.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Pandora.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiVersion("1.0")]
    [ApiExceptionFilter]
    public class DishesController : ControllerBase
    {
        private readonly IDishService DishService;

        public DishesController(
            IDishService DishService)
        {
            this.DishService = DishService;
        }

        [HttpGet]
        //[JwtAuthorize("Hydra.Accounts.Read")]
        [ProducesResponseType(typeof(Response<DishViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<Response<DishViewModel>> GetAsync([FromQuery] DishRequest request)
        {
            return await DishService.GetAsync(request).ConfigureAwait(false);
        }
        [HttpPost]
        //[JwtAuthorize("Hydra.Accounts.Read")]
        [ProducesResponseType(typeof(Result<DishResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAsync([FromBody] DishViewModelCreate request)
        {
            var result = await DishService.CreateAsync(request).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPut()]
        //[JwtAuthorize("Hydra.Accounts.Read")]
        [ProducesResponseType(typeof(UpdateResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<UpdateResult> PutAsync([FromBody, Required] DishUpdateViewModel dish)
        {
            return await DishService.PutAsync(dish).ConfigureAwait(false);
        }

        [HttpDelete()]
        //[JwtAuthorize("Hydra.Accounts.Read")]
        [ProducesResponseType(typeof(UpdateResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<UpdateResult> DeleteAsync([FromQuery, Required] int dishId)
        {
            return await DishService.DeleteAsync(dishId).ConfigureAwait(false);
        }

        [HttpGet("{dishId}/ingredients")]
        //[JwtAuthorize("Hydra.Accounts.Read")]
        [ProducesResponseType(typeof(DishDetailViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IEnumerable<DishDetailViewModel>> GetDetailAsync([FromRoute] int dishId)
        {
            return await DishService.GetDetailAsync(dishId).ConfigureAwait(false);
        }

        [HttpPut("details")]
        //[JwtAuthorize("Hydra.Accounts.Read")]
        [ProducesResponseType(typeof(UpdateResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<UpdateResult> PutDetailAsync([FromBody] DishDetailRequest details)
        {
            return await DishService.PutDetailAsync(details).ConfigureAwait(false);
        }

        [HttpDelete("details")]
        //[JwtAuthorize("Hydra.Accounts.Read")]
        [ProducesResponseType(typeof(UpdateResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<UpdateResult> DeleteDetailAsync([FromQuery, Required] int dishDetailId)
        {
            return await DishService.DeleteDetailAsync(dishDetailId).ConfigureAwait(false);
        }

        [HttpPost("Details")]
        //[JwtAuthorize("Hydra.Accounts.Read")]
        [ProducesResponseType(typeof(Result<DishResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateDetailAsync([FromBody] DishDetailViewModelCreate request)
        {
            var result = await DishService.CreateDetailAsync(request).ConfigureAwait(false);
            return Ok(result);
        }

    }
}