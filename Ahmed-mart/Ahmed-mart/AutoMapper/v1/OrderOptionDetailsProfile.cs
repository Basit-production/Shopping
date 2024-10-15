using Ahmed_mart.Dtos.v1.OrderOptionDetailsDto;
using Ahmed_mart.Models.v1;
using AutoMapper;

namespace Ahmed_mart.AutoMapper.v1
{
    public class OrderOptionDetailsProfile : Profile
    {
        public OrderOptionDetailsProfile()
        {
            CreateMap<OrderOptionDetails, GetOrderOptionDetailsDto>().ReverseMap();
            CreateMap<AddOrderOptionDetailsDto, OrderOptionDetails>().ReverseMap();
        }
    }
}
