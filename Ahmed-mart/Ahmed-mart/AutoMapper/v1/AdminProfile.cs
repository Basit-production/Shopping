using Ahmed_mart.Dtos.v1.AdminDtos;
using Ahmed_mart.Models.v1;
using AutoMapper;

namespace Ahmed_mart.AutoMapper.v1
{
    public class AdminProfile : Profile
    {
        public AdminProfile()
        {
            CreateMap<Admin, GetAdminDto>().ReverseMap();
            CreateMap<AddAdminDto, Admin>().ReverseMap();
            CreateMap<UpdateAdminDto, Admin>().ReverseMap();
        }
    }
}
