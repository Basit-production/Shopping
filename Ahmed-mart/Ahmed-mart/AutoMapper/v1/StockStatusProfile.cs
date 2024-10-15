using Ahmed_mart.Dtos.v1.StockStatusDto;
using Ahmed_mart.Models.v1;
using AutoMapper;

namespace Ahmed_mart.AutoMapper.v1
{
    public class StockStatusProfile : Profile
    {
        public StockStatusProfile()
        {
            CreateMap<StockStatus, GetStockStatusDto>().ReverseMap();
            CreateMap<AddStockStatusDto, StockStatus>().ReverseMap();
        }
    }
}
