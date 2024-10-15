using Ahmed_mart.Repository.v1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ahmed_mart.Controllers.v1
{
   
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected async Task<IActionResult> HandleServiceResponseAsync<T>(Task<ServiceResponse<T>> serviceTask)
        {
            var serviceResponse = await serviceTask;
            if (!serviceResponse.Success)
            {
                return BadRequest(serviceResponse);
            }
            return Ok(serviceResponse);
        }
    }
}
