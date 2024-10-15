using Ahmed_mart.Dtos.v1.OrderStatusDto;
using Ahmed_mart.Models.v1;
using AutoMapper;

namespace Ahmed_mart.AutoMapper.v1
{
    public class OrdersStatusProfile : Profile
    {
        public OrdersStatusProfile()
        {
            CreateMap<OrdersStatus, GetOrderStatusDto>().ReverseMap();
            CreateMap<AddOrderStatusDto, OrdersStatus>().ReverseMap();
            //CreateMap<OrdersStatus, GetOrderStatusDto>()
            //    .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.Status));
            //CreateMap<AddOrderStatusDto, OrdersStatus>()
            //    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.PaymentStatus));
        }
    }
}
