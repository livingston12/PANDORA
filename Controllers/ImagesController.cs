
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pandora.Core.Attributes;
using Pandora.Core.Interfaces;
using Pandora.Core.ViewModels;
using Pandora.Managers;
using System.Threading.Tasks;

namespace Pandora.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiVersion("1.0")]
    [ApiExceptionFilter]
    public class ImagesController : ControllerBase
    {
        private readonly IImageService ImageService;

        public ImagesController(
            IImageService ImageService
            )
        {
            this.ImageService = ImageService;
        }

        [HttpGet("{directory}/{name}")]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAsync(string directory, string name)
        {
            try
            {
                var result = await ImageService.GetAsync(directory, name);
                var contentType = $"image/${result.extention}";
                return File(result.file, contentType);
            }
            catch (System.Exception ex)
            {
                return Content(ex.Message);
            }

        }

        [HttpPost()]
        [ProducesResponseType(typeof(ImageViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ImageViewModel> PostAsync([FromForm] FileUpload files, string directory)
        {
            return await ImageService.PostAsync(files, directory);
        }

    }
}