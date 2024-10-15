using Ahmed_mart.Dtos.v1.LoginDtos;
using Ahmed_mart.Dtos.v1.OtpDto;
using Ahmed_mart.Dtos.v1.UserDtos;
using Ahmed_mart.Services.v1.UserAuthService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ahmed_mart.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize]
    public class UserAuthController :  BaseController
    {
        private readonly IUserAuthService _userAuthService;
        public UserAuthController(IUserAuthService userAuthService)
        {
            _userAuthService = userAuthService;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(GetLoginDto loginDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_userAuthService.Login(loginDto));
            return serviceResponse;
        }

        [AllowAnonymous]
        [HttpPost("AddUser")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddUser([FromForm] AddUserDto addUserDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_userAuthService.AddUser(addUserDto));
            return serviceResponse;
        }

        [HttpPut("UpdateUser")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateUser([FromForm] UpdateUserDto updateUserDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_userAuthService.UpdateUser(updateUserDto));
            return serviceResponse;
        }

        [HttpPut("LockUnlockUser/{Id:int}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LockUnlockUser(int Id)
        {
            var serviceResponse = await HandleServiceResponseAsync(_userAuthService.LockUnlockUser(Id));
            return serviceResponse;
        }

        [HttpPut("UpdateUserStatus/{Id:int}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateUserStatus(int Id)
        {
            var serviceResponse = await HandleServiceResponseAsync(_userAuthService.UpdateUserStatus(Id));
            return serviceResponse;
        }

        [HttpDelete("DeleteUser/{Id:int}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteUser(int Id)
        {
            var serviceResponse = await HandleServiceResponseAsync(_userAuthService.DeleteUser(Id));
            return serviceResponse;
        }

        [AllowAnonymous]
        [HttpPost("ValidateEmail")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ValidateEmail(AddOtpDto addOtpDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_userAuthService.ValidateEmail(addOtpDto));
            return serviceResponse;
        }

        [AllowAnonymous]
        [HttpPost("ValidateEmailForPasswordRecovery")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ValidateEmailForPasswordRecovery(AddOtpDto addOtpDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_userAuthService.ValidateEmailForPasswordRecovery(addOtpDto));
            return serviceResponse;
        }

        [AllowAnonymous]
        [HttpPut("UpdatePassword")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdatePassword(UpdateUserDto updateUserDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_userAuthService.UpdatePassword(updateUserDto));
            return serviceResponse;
        }

        [AllowAnonymous]
        [HttpGet("LogOut")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LogOut()
        {
            var serviceResponse = await HandleServiceResponseAsync(_userAuthService.LogOut());
            return serviceResponse;
        }

        [AllowAnonymous]
        [HttpPost("LoginThroughMobiles")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LoginThroughMobile(GetLoginThroughMobileDto loginDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_userAuthService.LoginThroughMobile(loginDto));
            return serviceResponse;
        }

        // GET /GetSingleGender/{id}
        [AllowAnonymous]
        [HttpGet("GetSingleOtp/{Id:int}")]
        public async Task<IActionResult> GetSingleOtp(int Id)
        {
            var serviceResponse = await HandleServiceResponseAsync(_userAuthService.GetSingleOtp(Id));
            return serviceResponse;
        }

        [AllowAnonymous]
        [HttpPost("VerifyOtpAndLogin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> VerifyOtpAndLogin(VerifyOtpDto verifyOtpDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_userAuthService.VerifyOtpAndLogin(verifyOtpDto));
            return serviceResponse;
        }

        [AllowAnonymous]
        [HttpPost("ResendOtp")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ResendOtp(ResendOtpDto rsndOtp)
        {
            var serviceResponse = await HandleServiceResponseAsync(_userAuthService.ResendOtp(rsndOtp));
            return serviceResponse;
        }
        // POST /RefreshToken
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var serviceResponse =await HandleServiceResponseAsync(_userAuthService.RefreshTokenAsync());
            return serviceResponse;
        }
    }
}
