using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NI_BE.DataDb.Models;
using NI_BE.Services;

namespace NI_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoaderController : ControllerBase
    {
        private LoaderService _loaderService;

        public LoaderController(LoaderService loaderService)
        {
            _loaderService = loaderService;
        }

        [HttpPost("load-file")]
        public async Task<IActionResult> LoadControllerPost([FromForm] FileUpload fileUpload)
        {
            var result = await _loaderService.UploadFile(fileUpload.File);

            if (result == "Invalid file")
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

    }
}
