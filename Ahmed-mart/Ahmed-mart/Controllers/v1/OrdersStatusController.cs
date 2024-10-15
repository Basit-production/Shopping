using Ahmed_mart.Dtos.v1.OrderHistoryDto;
using Ahmed_mart.Dtos.v1.OrderStatusDto;
using Ahmed_mart.Services.v1.OrdersStatusService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ahmed_mart.Controllers.v1
{
    [Authorize]
    [ApiVersion("1.0")]
    public class OrdersStatusController : BaseController
    {
        private readonly IOrdersStatusService _ordersStatusService;

        public OrdersStatusController(IOrdersStatusService ordersStatusService)
        {
            _ordersStatusService = ordersStatusService;
        }

        [HttpGet("GetOrdersStatus")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetOrdersStatus()
        {
            var serviceResponse = await HandleServiceResponseAsync(_ordersStatusService.GetOrdersStatus());
            return serviceResponse;
        }

        [HttpGet("GetOrderStatus/{Id:int}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetOrderStatus(int Id)
        {

            var serviceresponse = await HandleServiceResponseAsync(_ordersStatusService.GetOrderStatus(Id));
            return serviceresponse;
        }

        [HttpPost("AddOrderStatus")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddOrderStatus([FromBody] AddOrderStatusDto addOrderStatusDto)
        {
            var serviceresponse = await HandleServiceResponseAsync(_ordersStatusService.AddOrderStatus(addOrderStatusDto));
            return serviceresponse;
        }

        [HttpPut("UpdateOrderStatus")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatusDto updateOrderStatusDto)
        {
            var serviceresponse = await HandleServiceResponseAsync(_ordersStatusService.UpdateOrderStatus(updateOrderStatusDto));
            return serviceresponse;
        }

        [HttpDelete("DeleteOrderStatus/{Id:int}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteOrderStatus(int Id)
        {
            var serviceresponse = await HandleServiceResponseAsync(_ordersStatusService.DeleteOrderStatus(Id));
            return serviceresponse;
        }
    }
}
