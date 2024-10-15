using Ahmed_mart.Dtos.v1.PrefixesDto;
using Ahmed_mart.Models.v1;
using AutoMapper;
namespace Ahmed_mart.AutoMapper.v1
{
    public class PrefixesProfile : Profile
    {
        public PrefixesProfile()
        {
            CreateMap<Prefixes, GetPrefixesDto>();
            CreateMap<AddPrefixesDto, Prefixes>();
        }
    }
}
