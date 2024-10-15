using Ahmed_mart.Dtos.v1.OrderPaymentsDto;
using Ahmed_mart.Models.v1;
using AutoMapper;

namespace Ahmed_mart.AutoMapper.v1
{
    public class OrderPaymentsProfile : Profile
    {
        public OrderPaymentsProfile()
        {
            CreateMap<OrderPayments, GetOrderPaymentsDto>().ReverseMap();
            CreateMap<AddOrderPaymentsDto, OrderPayments>().ReverseMap();
        }
    }
}
