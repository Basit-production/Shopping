using Ahmed_mart.Dtos.v1.GetOrderByStoreDtos;
using Ahmed_mart.Dtos.v1.OrdersDto;
using Ahmed_mart.Services.v1.OrdersService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ahmed_mart.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin")]
    public class OrdersController : BaseController
    {
       private readonly IOrdersService _ordersService;

        public OrdersController(IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }
        [HttpGet("GetOrders")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task <IActionResult> GetOrders()
        {
            var serviceResponse = await HandleServiceResponseAsync(_ordersService.GetOrders());
            return serviceResponse;
        }

        [HttpGet("GetOrder/{Id:int}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrder(int Id)
        {
            var serviceResponse=await HandleServiceResponseAsync(_ordersService.GetOrder(Id));
            return serviceResponse;
        }

        //[HttpPost("AddOrder")]
        //[ProducesResponseType(StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> AddOrder([FromBody] AddOrdersDto addOrdersDto)
        //{
        //    var command = new AddOrdersCommand(addOrdersDto);
        //    var serviceResponse = await _mediator.Send(command);
        //    if (serviceResponse.Success == false)
        //    {
        //        return BadRequest(serviceResponse);
        //    }
        //    return Ok(serviceResponse);
        //}

        [HttpPut("UpdateOrder")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrder([FromBody] UpdateOrdersDto updateOrdersDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_ordersService.UpdateOrder(updateOrdersDto));
            return serviceResponse;
        }

        [HttpDelete("DeleteOrder/{Id:int}")]
        public async Task<IActionResult> DeleteOrder(int Id)
        {
            var serviceResponse = await HandleServiceResponseAsync(_ordersService.DeleteOrder(Id));
            return serviceResponse;
        }

        //[HttpGet("GetOrders")]
        ////[MapToApiVersion("1.1")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> GetEmployees()
        //{
        //    var query = new GetOrdersQuery();
        //    var serviceResponse = await _mediator.Send(query);
        //    if (serviceResponse.Success == false)
        //    {
        //        return BadRequest(serviceResponse);
        //    }
        //    return Ok(serviceResponse);
        //}

        [HttpPost("GetOrdersByStore")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetOrdersByStore([FromBody] GetOrderByStoreFilterDto addOrdersDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_ordersService.GetOrdersByStore(addOrdersDto));
            return serviceResponse;
        }

        [HttpGet("GetRecentBuyersByStore/{StoreId:int}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetRecentBuyersByStore(int StoreId)
        {
            var serviceResponse = await HandleServiceResponseAsync(_ordersService.GetRecentBuyersByStore(StoreId));
            return serviceResponse;
        }

        [HttpPost("GetCustomerSalesReportByStore")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCustomerSalesReportByStore([FromBody] GetOrderByStoreFilterDto addOrdersDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_ordersService.GetCustomerSalesReportByStore(addOrdersDto));
            return serviceResponse;
        }

        [HttpPost("GetSoldProductsReportByStore")]
        public async Task<IActionResult> GetSoldProductsReportByStore([FromBody] GetOrderByStoreFilterDto addOrdersDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_ordersService.GetSoldProductsReportByStore(addOrdersDto));
            return serviceResponse;
        }

        [HttpPost("GetSalesReportByStore")]
        public async Task<IActionResult> GetSalesReportByStore([FromBody] GetOrderByStoreFilterDto addOrdersDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_ordersService.GetSalesReportByStore(addOrdersDto));
            return serviceResponse;
        }

        [HttpPost("GetBestSellingProductsByStore")]
        public async Task<IActionResult> GetBestSellingProductsByStore([FromBody] AddOrdersDto addOrdersDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_ordersService.GetBestSellingProductsByStore(addOrdersDto));
            return serviceResponse;
        }

        [HttpGet("GetTotalOrdersByStore/{StoreId:int}")]
        public async Task<IActionResult> GetTotalOrdersByStore(int StoreId)
        {
            var serviceResponse = await HandleServiceResponseAsync(_ordersService.GetTotalOrdersByStore(StoreId));
            return serviceResponse;
        }

        [HttpGet("GetSoldProductsByStore/{StoreId:int}")]
        public async Task<IActionResult> GetSoldProductsByStore(int StoreId)
        {
            var serviceResponse = await HandleServiceResponseAsync(_ordersService.GetSoldProductsByStore(StoreId));
            return serviceResponse;
        }

        //[HttpPost("CaptureRazorpayPayment/{key}/{secret}/{razorPayPaymentId}/{orderId}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> CaptureRazorpayPayment([FromRoute] string key, [FromRoute] string secret, [FromRoute] string razorPayPaymentId, [FromRoute] int orderId)
        //{
        //    var serviceResponse = _paymentGatewayServiceRepo.CaptureRazorpayPayment(key, secret, razorPayPaymentId);

        //    //update order payment table
        //    var orderPaymentInfo = await _orderPaymentsRepo.Search(x => x.OrdersId == orderId);
        //    var orderPayment = orderPaymentInfo.FirstOrDefault();
        //    orderPayment.RazorpayPaymentId = razorPayPaymentId;
        //    orderPayment.RazorpayPaymentStatus = serviceResponse.Data["status"].ToString();
        //    orderPayment.ModifiedAt = DateTime.Now;

        //    orderPayment.ModifiedBy = GetUserId();
        //    _orderPaymentsRepo.Update(orderPayment);
        //    await _unitOfWork.CommitAsync();

        //    if (serviceResponse.Success == false)
        //    {
        //        return BadRequest(serviceResponse);
        //    }
        //    return Ok(serviceResponse);
        //}

        //[HttpGet("GetOrdersByCustomerAndStore/{StoreId:int}/{CustomersId:int}")]
        //public async Task<IActionResult> GetOrdersByCustomerAndStore(int StoreId,int CustomersId)
        //{
        //    var serviceResponse = await HandleServiceResponseAsync(_ordersService.GetOrdersByCustomerAndStore(StoreId, CustomersId));
        //    return serviceResponse;
        //}
    }
}
