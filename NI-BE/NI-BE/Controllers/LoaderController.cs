using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NI_BE.DataDb.Models;
using NI_BE.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            string fileName = Path.GetFileName(fileUpload.File.FileName);

            string uploadDirectory = Path.Combine("C:\\Users\\User\\Desktop\\G\\Baby NI Project\\Code\\NI-BE\\NI-BE\\NI-BE\\Data\\", "Uploads");

            if(!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }
            
            string filePath = Path.Combine(uploadDirectory, fileName);

            using(var stream = new FileStream(filePath, FileMode.Create))
            {
                await fileUpload.File.CopyToAsync(stream);
            }


            var result = await _loaderService.UploadFile(filePath);

            if (result == "Invalid file")
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

    }
}
