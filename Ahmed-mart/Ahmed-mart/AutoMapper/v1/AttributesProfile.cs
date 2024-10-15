using Ahmed_mart.Dtos.v1.AttributesDto;
using Ahmed_mart.Models.v1;
using AutoMapper;

namespace Ahmed_mart.AutoMapper.v1
{
    public class AttributesProfile : Profile
    {
        public AttributesProfile()
        {
            CreateMap<Attributes, GetAttributesDto>().ReverseMap();
            CreateMap<AddAttributesDto, Attributes>().ReverseMap();
        }
    }
}

