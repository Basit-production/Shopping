using Ahmed_mart.Dtos.v1.PriceListDto;
using Ahmed_mart.Models.v1;
using AutoMapper;

namespace Ahmed_mart.AutoMapper.v1
{
    public class PriceListProfile : Profile
    {
        public PriceListProfile()
        {
            CreateMap<PriceList, GetPriceListDto>().ReverseMap();
            CreateMap<AddPriceListDto, PriceList>().ReverseMap();
        }
    }
}
