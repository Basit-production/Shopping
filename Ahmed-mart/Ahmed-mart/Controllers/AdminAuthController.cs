using Ahmed_mart.Controllers.v1;
using Ahmed_mart.Dtos.v1.AdminDtos;
using Ahmed_mart.Dtos.v1.LoginDtos;
using Ahmed_mart.Dtos.v1.OtpDto;
using Ahmed_mart.Dtos.v1.UserDtos;
using Ahmed_mart.Services.v1.AdminAuthService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ahmed_mart.Controllers
{
   
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin")]
    //[Route("api/[controller]")]
    //[ApiController]
    public class AdminAuthController : BaseController
    {
        private readonly IAdminAuthService _adminAuthService;
        public AdminAuthController(IAdminAuthService adminAuthService)
        {
            _adminAuthService = adminAuthService;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] GetLoginDto loginDto)
        {
            var serviceResponse=  await HandleServiceResponseAsync(_adminAuthService.Login(loginDto));
            return serviceResponse;
        }

        [HttpPost("AddAdmin")]
        //public async Task<IActionResult> AddAdmin([FromBody] AddAdminDto addAdminDto)
        public async Task<IActionResult> AddAdmin([FromForm] AddAdminDto addAdminDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_adminAuthService.AddAdmin(addAdminDto));
            return serviceResponse;     
        }

        [HttpPut("UpdateAdmin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateAdmin([FromForm] UpdateAdminDto updateAdminDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_adminAuthService.UpdateAdmin(updateAdminDto));
            return serviceResponse;
        }

        [HttpPut("LockUnlockAdmin/{Id:int}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LockUnlockAdmin(int Id)
        {
            var serviceResponse = await HandleServiceResponseAsync(_adminAuthService.LockUnlockAdmin(Id));
            return serviceResponse;
        }

        [HttpPut("UpdateAdminStatus/{Id:int}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateAdminStatus(int Id)
        {
            var ServiceResponse = await HandleServiceResponseAsync(_adminAuthService.UpdateAdminStatus(Id));
            return ServiceResponse;
        }

        [HttpDelete("DeleteAdmin/{Id:int}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteAdmin(int Id)
        {
            var ServiceResponse = await HandleServiceResponseAsync(_adminAuthService.DeleteAdmin(Id));
            return ServiceResponse;
        }

        [AllowAnonymous]
        [HttpPost("ValidateEmailForPasswordRecovery")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ValidateEmailForPasswordRecovery([FromForm] AddOtpDto addOtpDto)
        {
            var ServiceResponse = await HandleServiceResponseAsync(_adminAuthService.ValidateEmailForPasswordRecovery(addOtpDto));
            return ServiceResponse;
        }
        [AllowAnonymous]
        [HttpPut("UpdatePassword")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdatePassword(UpdateAdminDto updateAdminDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_adminAuthService.UpdatePassword(updateAdminDto));
            return serviceResponse;
        }
        [AllowAnonymous]
        [HttpGet("LogOut")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LogOut()
        {
            var serviceResponse = await HandleServiceResponseAsync(_adminAuthService.LogOut());
            return serviceResponse;
        }
        //***********************
        [AllowAnonymous]
        [HttpPost("LoginThroughMobiles")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LoginThroughMobile(GetLoginThroughMobileDto loginDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_adminAuthService.LoginThroughMobile(loginDto));
            return serviceResponse;
        }
        [AllowAnonymous]
        [HttpPost("VerifyOtpAndLogin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> VerifyOtpAndLogin(VerifyOtpDto verifyOtpDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_adminAuthService.VerifyOtpAndLogin(verifyOtpDto));
            return serviceResponse;
        }
        [AllowAnonymous]
        [HttpPost("ResendOtp")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ResendOtp(ResendOtpDto rsndOtp)
        {
            var serviceResponse = await HandleServiceResponseAsync(_adminAuthService.ResendOtp(rsndOtp));
            return serviceResponse;
        }
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var serviceResponse = await HandleServiceResponseAsync(_adminAuthService.RefreshTokenAsync());
            return serviceResponse;
        }
    }
}
