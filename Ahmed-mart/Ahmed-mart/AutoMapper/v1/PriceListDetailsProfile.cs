using Ahmed_mart.Dtos.v1.PriceListDetailsDto;
using Ahmed_mart.Models.v1;
using AutoMapper;

namespace Ahmed_mart.AutoMapper.v1
{
    public class PriceListDetailsProfile : Profile
    {
        public PriceListDetailsProfile()
        {
            CreateMap<PriceListDetails, GetPriceListDetailsDto>().ReverseMap();
            CreateMap<AddPriceListDetailsDto, PriceListDetails>().ReverseMap();
        }
    }
}
