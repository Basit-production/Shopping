using Ahmed_mart.Dtos.v1.RoleDtos;
using Ahmed_mart.Models.v1;
using AutoMapper;

namespace Ahmed_mart.AutoMapper.v1
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Role, GetRoleDto>().ReverseMap();
            //CreateMap<AddRoleDto, Role>();
        }
    }
}
