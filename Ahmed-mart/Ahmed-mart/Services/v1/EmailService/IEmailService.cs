using Ahmed_mart.Dtos.v1.EmailDto;
using Ahmed_mart.Repository.v1;

namespace Ahmed_mart.Services.v1.EmailService
{
    public interface IEmailService
    {
        Task<ServiceResponse<GetEmailDto>> SendEmailAsync(GetEmailDto getEmailDto);
    }
}
