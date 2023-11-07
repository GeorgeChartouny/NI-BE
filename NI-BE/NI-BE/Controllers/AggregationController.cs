using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NI_BE.DataDb.Models;
using NI_BE.Services;

namespace NI_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AggregationController : ControllerBase
    {
        private AggregationService _aggregationService;

        public AggregationController(AggregationService aggregationService)
        {
            _aggregationService = aggregationService;
        }

        [HttpPost("aggregate-file")]
        public async Task<IActionResult> AggregateControllerPost([FromForm] FileUpload fileUpload)
        {
            var result = await _aggregationService.UploadFile(fileUpload.File);

            if (result == "Invalid file")
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
