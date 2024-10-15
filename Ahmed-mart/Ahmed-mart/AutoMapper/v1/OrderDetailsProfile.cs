using Ahmed_mart.Dtos.v1.OrderDetailsDto;
using Ahmed_mart.Models.v1;
using AutoMapper;

namespace Ahmed_mart.AutoMapper.v1
{
    public class OrderDetailsProfile : Profile
    {
        public OrderDetailsProfile()
        {
            CreateMap<OrderDetails, GetOrderDetailsDto>().ReverseMap();
            CreateMap<AddOrderDetailsDto, OrderDetails>().ReverseMap();
        }
    }
}
