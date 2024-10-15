

using Ahmed_mart.Dtos.v1.StoreDto;
using Ahmed_mart.Models.v1;
using AutoMapper;

namespace Ahmed_mart.AutoMapper.v1
{
    public class StoreProfile : Profile
    {
        public StoreProfile()
        {
            CreateMap<Store, GetStoreDto>().ReverseMap();
            CreateMap<AddStoreDto, Store>().ReverseMap();
        }
    }
}
