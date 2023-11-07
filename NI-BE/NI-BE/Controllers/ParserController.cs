using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NI_BE.DataDb.Models;
using NI_BE.Services;

namespace NI_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParserController : ControllerBase
    {
        private ParserService _parserService;


        public ParserController(ParserService parserService)
        {
            _parserService = parserService;
        }

        [HttpPost("parse-file")]
        public async Task<IActionResult> ParseControllerPost([FromForm] FileUpload fileUpload)
        {
            var result = await _parserService.UploadFile(fileUpload.File);

            if(result == "Invalid file") {
            return BadRequest(result);
            }

            return Ok(result);

        }

    }
}
