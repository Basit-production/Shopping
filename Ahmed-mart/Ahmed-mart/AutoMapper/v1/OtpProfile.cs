using Ahmed_mart.Dtos.v1.OtpDto;
using Ahmed_mart.Models.v1;
using AutoMapper;

namespace Ahmed_mart.AutoMapper.v1
{
    public class OtpProfile : Profile
    {
        public OtpProfile()
        {
            CreateMap<Otp, GetOtpDto>();
            CreateMap<AddOtpDto, Otp>();
        }
    }
}
