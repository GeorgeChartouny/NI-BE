using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult ParseControllerPost([FromBody] FileSystemEventArgs e)
        {
            _parserService.ParseAFile(e);
            return Ok();
        }
    }
}
