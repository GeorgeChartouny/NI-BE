using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NI_BE.DataDb.Models;
using NI_BE.Services;

namespace NI_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetDataController : ControllerBase
    {
        private GetDataService _getDataService;

        public GetDataController(GetDataService getDataService)
        {
            _getDataService = getDataService;
        }

        [HttpPost("get-data")]
        public IActionResult DataGetController([FromBody]GetDataModel getDataModel) 
        {
         if(getDataModel != null)
            {

            var getDataFromDatabase = _getDataService.GetData(getDataModel);
            if(getDataFromDatabase != null)
            {

            return Ok(getDataFromDatabase);
            }else
            {
                return BadRequest();
            }
            }   else
            {
                return BadRequest("Invalid data format.");
            }
        }
    }
}
