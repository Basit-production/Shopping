using Ahmed_mart.Controllers.v1;
using Ahmed_mart.Dtos.v1.StoreTypeDto;
using Ahmed_mart.Services.v1.StoreTypeService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace hmed_mart.Controllers.v1
{
   // [Authorize]
    [ApiVersion("1.0")]
    public class StoreTypeController : BaseController
    {
        private readonly IStoreTypeService _storeTypeService;
        public StoreTypeController(IStoreTypeService storeTypeService)
        {
            _storeTypeService = storeTypeService;
        }

        [HttpGet("GetStoreTypes")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetStoreTypes()
        {
            var serviceResponse = await HandleServiceResponseAsync(_storeTypeService.GetStoreTypes());
            return serviceResponse;
        }
        
        [AllowAnonymous]
        [HttpGet("GetStoreTypesForStoreFront")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetStoreTypesForStoreFront()
        {
            var serviceResponse = await HandleServiceResponseAsync(_storeTypeService.GetStoreTypesForStoreFront());
            return serviceResponse;
        }

        [AllowAnonymous]
        [HttpPost("AddStoreType")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddStoreType([FromBody]AddStoreTypeDto addstoreTypeDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_storeTypeService.AddStoreType(addstoreTypeDto));
            return serviceResponse;
        }
        [AllowAnonymous]
        [HttpPut("UpdateStoreType")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateStoreType([FromBody] UpdateStoreTypeDto updatestoreTypeDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_storeTypeService.UpdateStoreType(updatestoreTypeDto));
            return serviceResponse;
        }
    }
}
