using Ahmed_mart.Controllers.v1;
using Ahmed_mart.Dtos.v1.StoreDto;
using Ahmed_mart.Services.v1.StoreService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace hmed_mart.Controllers.v1
{
    //[Authorize]
    [ApiVersion("1.0")]
    public class StoreController : BaseController
    {
        private readonly IStoreService _storeService;
        //private readonly IQRcodeService _qRcodeService;
        public StoreController(
            IStoreService storeService
            //IQRcodeService qRcodeService
            )
        {
            _storeService = storeService;
            //_qRcodeService = qRcodeService;
        }
        [AllowAnonymous]
        [HttpGet("GetStores")]//1
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetStores()
        {
            var serviceResponse = await HandleServiceResponseAsync(_storeService.GetStores());
            return serviceResponse;
        }

        [HttpGet("GetStore/{Id:int}")]//2
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetStore(int Id)
        {
            var serviceResponse = await HandleServiceResponseAsync(_storeService.GetStore(Id));
            return serviceResponse;
        }

        [AllowAnonymous]
        [HttpGet("GetUserStores/{UserId:int}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserStores(int UserId)
        {
           var serviceResponse=await HandleServiceResponseAsync(_storeService.GetUserStores(UserId));
           return serviceResponse;
        }

        [HttpPost("AddStore")]//3
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddStore([FromForm] AddStoreDto addStoreDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_storeService.AddStore(addStoreDto));
            return serviceResponse;
        }

        [HttpPut("UpdateStore")]//4
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStore([FromBody] UpdateStoreDto updateStoreDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_storeService.UpdateStore(updateStoreDto));
            return serviceResponse;
        }

        [HttpDelete("DeleteStore/{Id:int}")]//5
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteStore(int Id)
        {
            var serviceResponse = await HandleServiceResponseAsync(_storeService.DeleteStore(Id));
            return serviceResponse;
        }

        [AllowAnonymous]
        [HttpGet("GetStoreByCode/{Code}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetStoreByCode(string code)
        {
            var serviceResponse = await HandleServiceResponseAsync(_storeService.GetStoreByCode(code));
            return serviceResponse;
        }

        //[AllowAnonymous]
        //[HttpGet("GetSubscriptions")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> GetSubscriptions()
        //{
        //    var query = new GetSubscriptionsQuery();
        //    var serviceResponse = await _mediator.Send(query);
        //    if (serviceResponse.Success == false)
        //    {
        //        return BadRequest(serviceResponse);
        //    }
        //    return Ok(serviceResponse);
        //}

        //[AllowAnonymous]
        //[HttpGet("GetStoreFront/{Id:int}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> GetStoreFront(int Id)
        //{
        //    var query = new GetStoreQuery(Id);
        //    var serviceResponse = await _mediator.Send(query);
        //    if (serviceResponse.Data == null)
        //    {
        //        return NotFound(serviceResponse);
        //    }
        //    if (serviceResponse.Success == false)
        //    {
        //        return BadRequest(serviceResponse);
        //    }

        //    return Ok(serviceResponse);
        //}

        //[HttpPost("GenerateQRcode")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public IActionResult GenerateQRcode([FromBody] GetQRcodeDto getQRcodeDto)
        //{
        //    var serviceResponse = _qRcodeService.GenerateQrcode(getQRcodeDto.QRcode);
        //    return Ok(serviceResponse);
        //}

        //[HttpGet("GetVendorStores/{VendorId:int}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> GetVendorStores(int VendorId)
        //{
        //    var query = new GetVendorStoresQuery(VendorId);
        //    var serviceResponse = await _mediator.Send(query);
        //    if (serviceResponse.Data == null)
        //    {
        //        return NotFound(serviceResponse);
        //    }
        //    if (serviceResponse.Success == false)
        //    {
        //        return BadRequest(serviceResponse);
        //    }
        //    return Ok(serviceResponse);
        //}

        //[AllowAnonymous]
        //[HttpGet("GetStoresByUser/{UserId:int}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> GetStoresByUser(int UserId)
        //{
        //    var query = new GetUserStoresQuery(UserId);
        //    var serviceResponse = await _mediator.Send(query);
        //    if (serviceResponse.Data == null)
        //    {
        //        return NotFound(serviceResponse);
        //    }
        //    if (serviceResponse.Success == false)
        //    {
        //        return BadRequest(serviceResponse);
        //    }

        //    return Ok(serviceResponse);
        //}

        //[AllowAnonymous]
        //[HttpGet("GetStoreByCode/{Code}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> GetStoreByCode(string Code)
        //{
        //    var query = new GetStoreByCodeQuery(Code);
        //    var serviceResponse = await _mediator.Send(query);
        //    if (serviceResponse.Data == null)
        //    {
        //        return NotFound(serviceResponse);
        //    }
        //    if (serviceResponse.Success == false)
        //    {
        //        return BadRequest(serviceResponse);
        //    }
        //    return Ok(serviceResponse);
        //}
        //[HttpGet("GetSmtpDetails/{StoreId:int}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> GetSmtpDetails(int StoreId)
        //{
        //    var query = new GetSmtpDetailsQuery(StoreId);
        //    var serviceResponse = await _mediator.Send(query);
        //    if (serviceResponse.Data == null)
        //    {
        //        return NotFound(serviceResponse);
        //    }
        //    if (serviceResponse.Success == false)
        //    {
        //        return BadRequest(serviceResponse);
        //    }
        //    return Ok(serviceResponse);
        //}

        //[HttpPost("AddSmtpDetails")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> AddSmtpDetails([FromBody] AddSmtpDetailsDto addSmtpDetailsDto)
        //{
        //    var command = new AddSmtpDetailsCommand(addSmtpDetailsDto);
        //    var serviceResponse = await _mediator.Send(command);
        //    if (serviceResponse.Data == null)
        //    {
        //        return NotFound(serviceResponse);
        //    }
        //    if (serviceResponse.Success == false)
        //    {
        //        return BadRequest(serviceResponse);
        //    }
        //    return Ok(serviceResponse);
        //}

        //[HttpPut("UpdateSmtpDetails")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> UpdateSmtpDetails([FromBody] UpdateSmtpDetailsDto updateSmtpDetailsDto)
        //{
        //    var command = new UpdateSmtpDetailsCommand(updateSmtpDetailsDto);
        //    var serviceResponse = await _mediator.Send(command);
        //    if (serviceResponse.Data == null)
        //    {
        //        return NotFound(serviceResponse);
        //    }
        //    if (serviceResponse.Success == false)
        //    {
        //        return BadRequest(serviceResponse);
        //    }
        //    return Ok(serviceResponse);
        //}
    }
}
