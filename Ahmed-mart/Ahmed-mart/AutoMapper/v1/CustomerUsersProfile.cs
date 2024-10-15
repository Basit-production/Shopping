using Ahmed_mart.Dtos.v1.CustomerUsersDto;
using Ahmed_mart.Models.v1;
using AutoMapper;

namespace Ahmed_mart.AutoMapper.v1
{
    public class CustomerUsersProfile : Profile
    {
        public CustomerUsersProfile()
        {
            CreateMap<CustomerUsers, GetCustomerUsersDto>().ReverseMap();
        }
    }
}
