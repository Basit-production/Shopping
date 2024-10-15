using Ahmed_mart.Controllers.v1;
using Ahmed_mart.Dtos.v1;
using Ahmed_mart.Dtos.v1.AdminDtos;
using Ahmed_mart.Dtos.v1.CustomersDto;
using Ahmed_mart.Dtos.v1.CustomerUsersDto;
using Ahmed_mart.Dtos.v1.LoginDtos;
using Ahmed_mart.Dtos.v1.OtpDto;
using Ahmed_mart.Services.v1.CustomerAuthService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ahmed_mart.Controllers
{
    [ApiVersion("1.0")]
    [Authorize]
    public class CustomerAuthController : BaseController
    {
        private readonly ICustomerAuthService _customerAuthService;

        public CustomerAuthController(ICustomerAuthService customerAuthService)
        {
            _customerAuthService = customerAuthService;
        }
        [AllowAnonymous]
        [HttpPost("CustomerLogin")]
        public async Task<IActionResult> CustomerLogin([FromBody] CustomerLoginDto customerLoginDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_customerAuthService.CustomerLogin(customerLoginDto));
            return serviceResponse;
        }
        [AllowAnonymous]
        [HttpPost("ValidateCustomerEmail")]
        public async Task<IActionResult> ValidateCustomerEmail(AddOtpDto addOtpDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_customerAuthService.ValidateCustomerEmail(addOtpDto));
            return serviceResponse;
        }
        [AllowAnonymous]
        [HttpPost("ValidateEmailForPasswordRecovery")]
        public async Task<IActionResult> ValidateEmailForPasswordRecovery(AddOtpDto addOtpDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_customerAuthService.ValidateEmailForPasswordRecovery(addOtpDto));
            return serviceResponse;
        }
        [AllowAnonymous]
        [HttpPut("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdateCustomerUsersDto updateCustomerUsersDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_customerAuthService.UpdatePassword(updateCustomerUsersDto));
            return serviceResponse;
        }
    }
}
