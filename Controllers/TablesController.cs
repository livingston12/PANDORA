
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
    public class TablesController : ControllerBase
    {
        private readonly ITableService TableService;

        public TablesController(
            ITableService TableService)
        {
            this.TableService = TableService;
        }

        [HttpGet]
        //[JwtAuthorize("Hydra.Accounts.Read")]
        [ProducesResponseType(typeof(Response<TableViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<Response<TableViewModel>> GetAsync([FromQuery] TableRequest request)
        {
            return await TableService.GetAsync(request).ConfigureAwait(false);
        }

        [HttpPost("reserved")]
        //[JwtAuthorize("Hydra.Accounts.Read")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ReservedAsync([FromBody] TableReservedRequest request)
        {
            var isReserved = await TableService.ReservedAsync(request.TableId).ConfigureAwait(false);
            return Ok(isReserved);
        }



    }
}