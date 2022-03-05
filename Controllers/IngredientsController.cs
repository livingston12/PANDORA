
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
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientService IngredientService;

        public IngredientsController(
            IIngredientService IngredientService
            )
        {
            this.IngredientService = IngredientService;
        }

        [HttpGet]
        //[JwtAuthorize("Hydra.Accounts.Read")]
        [ProducesResponseType(typeof(Response<IngredientViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<Response<IngredientViewModel>> GetAsync([FromQuery] IngredientRequest request)
        {
            return await IngredientService.GetAsync(request).ConfigureAwait(false);
        }

        [HttpGet("summary/{restaurantId}")]
        //[JwtAuthorize("Hydra.Accounts.Read")]
        [ProducesResponseType(typeof(IEnumerable<IngredientViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IEnumerable<IngredientViewModel>> GetSummaryAsync([FromRoute] int restaurantId)
        {
            return await IngredientService.GetSummaryAsync(restaurantId).ConfigureAwait(false);
        }

        [HttpGet("garrisons/{restaurantId}")]
        //[JwtAuthorize("Hydra.Accounts.Read")]
        [ProducesResponseType(typeof(IEnumerable<IngredientViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IEnumerable<IngredientViewModel>> GetGarrisonsAsync([FromRoute] int restaurantId)
        {
            return await IngredientService.GetGarrisonsAsync(restaurantId).ConfigureAwait(false);
        }

        [HttpPost]
        //[JwtAuthorize("Hydra.Accounts.Read")]
        [ProducesResponseType(typeof(Result<IngredientResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAsync([FromBody] IngredientViewModel request)
        {
            var result = await IngredientService.CreateAsync(request).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPut()]
        //[JwtAuthorize("Hydra.Accounts.Read")]
        [ProducesResponseType(typeof(UpdateResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<UpdateResult> PutAsync([FromBody, Required] IngredientViewModel ingredient)
        {
            return await IngredientService.PutAsync(ingredient).ConfigureAwait(false);
        }

        [HttpPut("inventory")]
        //[JwtAuthorize("Hydra.Accounts.Read")]
        [ProducesResponseType(typeof(UpdateResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<UpdateResult> PutInventoryAsync([FromBody, Required] IngredientUpdateInventory dish)
        {
            return await IngredientService.PutInventoryAsync(dish).ConfigureAwait(false);
        }

    }
}