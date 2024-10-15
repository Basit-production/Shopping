using Ahmed_mart.Dtos.v1.SmsDtos;
using Ahmed_mart.Repository.v1;

namespace Ahmed_mart.Services.v1.SmsService
{
    public interface ISmsService
    {
        Task<ServiceResponse<GetSmsDto>> SendSmsAsync(GetSmsDto getSmsDto);
    }
}
