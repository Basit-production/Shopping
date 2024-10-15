using Ahmed_mart.Dtos.v1.CustomersDto;
using Ahmed_mart.Models.v1;
using AutoMapper;

namespace Ahmed_mart.AutoMapper.v1
{
    public class CustomersProfile : Profile
    {
        public CustomersProfile()
        {
            CreateMap<Customers, GetCustomersDto>().ReverseMap();
            CreateMap<AddCustomersDto, Customers>().ReverseMap();
        }
    }
}
