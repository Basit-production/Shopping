using Ahmed_mart.Dtos.v1.OrderHistoryDto;
using Ahmed_mart.Models.v1;
using AutoMapper;

namespace Ahmed_mart.AutoMapper.v1
{
    public class OrderHistoryProfile : Profile
    {
        public OrderHistoryProfile()
        {
            CreateMap<OrderHistory, GetOrderHistoryDto>().ReverseMap();
            CreateMap<AddOrderHistoryDto, OrderHistory>().ReverseMap();
        }
    }
}
