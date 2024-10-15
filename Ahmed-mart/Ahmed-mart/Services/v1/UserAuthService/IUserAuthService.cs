using Ahmed_mart.Dtos.v1.LoginDtos;
using Ahmed_mart.Dtos.v1.OtpDto;
using Ahmed_mart.Dtos.v1.UserDtos;
using Ahmed_mart.Repository.v1;

namespace Ahmed_mart.Services.v1.UserAuthService
{
    public interface IUserAuthService
    {
        Task<ServiceResponse<GetUserDto>> Login(GetLoginDto loginDto);
        Task<ServiceResponse<GetUserDto>> AddUser(AddUserDto addUserDto);
        Task<ServiceResponse<GetUserDto>> UpdateUser(UpdateUserDto updateUserDto);
        Task<bool> UserExists(int Id, string email);
        Task<ServiceResponse<GetUserDto>> LockUnlockUser(int Id);
        Task<ServiceResponse<GetUserDto>> UpdateUserStatus(int Id);
        Task<ServiceResponse<GetUserDto>> DeleteUser(int Id);
        Task<ServiceResponse<GetOtpDto>> ValidateEmail(AddOtpDto addOtpDto);
        Task<ServiceResponse<GetOtpDto>> ValidateEmailForPasswordRecovery(AddOtpDto addOtpDto);
        Task<ServiceResponse<GetUserDto>> UpdatePassword(UpdateUserDto updateUserDto);
        Task<ServiceResponse<bool>> LogOut();
        Task<ServiceResponse<string>> LoginThroughMobile(GetLoginThroughMobileDto loginDto);
        Task<ServiceResponse<GetOtpDto>> GetSingleOtp(int Id);
        Task<ServiceResponse<GetUserDto>> VerifyOtpAndLogin(VerifyOtpDto verifyOtpDto);
        Task<ServiceResponse<GetOtpDto>> ResendOtp(ResendOtpDto rsndOtp);
        Task<ServiceResponse<bool>> RefreshTokenAsync();
    }
}
