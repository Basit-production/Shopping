using Ahmed_mart.Dtos.v1.OrderHistoryDto;
using Ahmed_mart.Services.v1.OrderHistoryService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ahmed_mart.Controllers.v1
{
    [Authorize]
    [ApiVersion("1.0")]
    public class OrderHistoryController : BaseController
    {
        private readonly IOrderHistoryService _orderHistoryService;

        public OrderHistoryController( IOrderHistoryService orderHistoryService)
        {
            _orderHistoryService = orderHistoryService;
        }

        [HttpPost("AddOrderHistory")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddOrderHistory([FromBody] AddOrderHistoryDto addOrderHistoryDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_orderHistoryService.AddOrderHistory(addOrderHistoryDto));
            return serviceResponse;
        }

        [HttpPut("UpdateOrderHistory")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateOrderHistory([FromBody] UpdateOrderHistoryDto updateOrderHistoryDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_orderHistoryService.UpdateOrderHistory(updateOrderHistoryDto));
            return serviceResponse;
        }
        //[HttpGet("GetOrderHistory")]
        //[ProducesResponseType(StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> GetOrderHistory()
        //{
        //    var serviceResponseAsync=await HandleServiceResponseAsync(_orderHistoryService.GetOrderHistory());
        //    return serviceResponseAsync;
        //}


    }
}
