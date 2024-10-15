using Ahmed_mart.Dtos.v1.AdminDtos;
using Ahmed_mart.Dtos.v1.CustomersDto;
using Ahmed_mart.Services.v1.CustomerService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ahmed_mart.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize]
    public class CustomersController : BaseController
    {
        private readonly ICustomersService _customersService;
        public CustomersController(ICustomersService customersService)
        {
            _customersService = customersService;
        }

        [HttpPost("AddCustomer")]
        public async Task<IActionResult> AddCustomer([FromForm] AddCustomersDto addCustomersDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_customersService.AddCustomer(addCustomersDto));
            return serviceResponse;
        }
        [HttpPut("UpdateCustomer")]
        public async Task<IActionResult> UpdateCustomer([FromForm] UpdateCustomersDto updateCustomersDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_customersService.UpdateCustomer(updateCustomersDto));
            return serviceResponse;
        }
        [HttpGet("GetCustomers")]
        public async Task<IActionResult> GetCustomers()
        {
            var serviceResponse=await  HandleServiceResponseAsync(_customersService.GetCustomers());
            return serviceResponse;
        }
    }
}
