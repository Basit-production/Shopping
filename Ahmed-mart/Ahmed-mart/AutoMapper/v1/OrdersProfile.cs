using Ahmed_mart.Dtos.v1.OrdersDto;
using Ahmed_mart.Models.v1;
using AutoMapper;

namespace Ahmed_mart.AutoMapper.v1
{
    public class OrdersProfile : Profile
    {
        public OrdersProfile()
        {
            CreateMap<Orders, GetOrdersDto>().ReverseMap();
            CreateMap<AddOrdersDto, Orders>().ReverseMap();
        }
    }
}
