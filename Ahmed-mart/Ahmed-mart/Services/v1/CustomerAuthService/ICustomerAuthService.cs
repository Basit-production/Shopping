using Ahmed_mart.Dtos.v1;
using Ahmed_mart.Dtos.v1.CustomersDto;
using Ahmed_mart.Dtos.v1.CustomerUsersDto;
using Ahmed_mart.Dtos.v1.OtpDto;
using Ahmed_mart.Repository.v1;

namespace Ahmed_mart.Services.v1.CustomerAuthService
{
    public interface ICustomerAuthService
    {
        Task<ServiceResponse<GetCustomerUsersDto>> CustomerLogin(CustomerLoginDto customerLoginDto);
        //Task<ServiceResponse<GetCustomerUsersDto>> AddCustomerUser(AddCustomerUsersDto addCustomerUsersDto);
        Task<ServiceResponse<GetOtpDto>> ValidateCustomerEmail(AddOtpDto addOtpDto);
        Task<ServiceResponse<GetOtpDto>> ValidateEmailForPasswordRecovery(AddOtpDto addOtpDto);
        Task<ServiceResponse<GetCustomerUsersDto>> UpdatePassword(UpdateCustomerUsersDto updateCustomerUsersDto);

    }
}
