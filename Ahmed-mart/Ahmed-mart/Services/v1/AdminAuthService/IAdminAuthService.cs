using Ahmed_mart.Dtos.v1.AdminDtos;
using Ahmed_mart.Dtos.v1.LoginDtos;
using Ahmed_mart.Dtos.v1.OtpDto;
using Ahmed_mart.Dtos.v1.UserDtos;
using Ahmed_mart.Repository.v1;

namespace Ahmed_mart.Services.v1.AdminAuthService
{
    public interface IAdminAuthService
    {
        Task<ServiceResponse<GetAdminDto>> Login(GetLoginDto loginDto);
        Task<ServiceResponse<GetAdminDto>> AddAdmin(AddAdminDto addAdminDto);
        Task<ServiceResponse<GetAdminDto>> UpdateAdmin(UpdateAdminDto updateAdminDto);
        Task<ServiceResponse<GetAdminDto>> LockUnlockAdmin(int Id);
        Task<ServiceResponse<GetAdminDto>> UpdateAdminStatus(int Id);
        Task<ServiceResponse<GetAdminDto>> DeleteAdmin(int id);
        Task<ServiceResponse<GetOtpDto>> ValidateEmailForPasswordRecovery(AddOtpDto addOtpDto);

        //Added
        Task<ServiceResponse<GetAdminDto>> UpdatePassword(UpdateAdminDto updateAdminDto);
        Task<ServiceResponse<bool>> LogOut();
        Task<ServiceResponse<string>> LoginThroughMobile(GetLoginThroughMobileDto loginDto);
        Task<ServiceResponse<GetAdminDto>> VerifyOtpAndLogin(VerifyOtpDto verifyOtpDto);
        Task<ServiceResponse<GetOtpDto>> ResendOtp(ResendOtpDto rsndOtp);
        Task<ServiceResponse<bool>> RefreshTokenAsync();
    }
}
