using Ahmed_mart.Dtos.v1.SmtpDetailsDto;
using Ahmed_mart.Models.v1;
using AutoMapper;

namespace Ahmed_mart.AutoMapper.v1
{
    public class SmtpDetailsProfile : Profile
    {
        public SmtpDetailsProfile()
        {
            CreateMap<SmtpDetails, GetSmtpDetailsDto>();
            CreateMap<AddSmtpDetailsDto, SmtpDetails>();
        }
    }
}
