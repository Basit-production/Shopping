
using Ahmed_mart.Dtos.v1.StoreTypeDto;
using Ahmed_mart.Models.v1;
using AutoMapper;

namespace Ahmed_mart.AutoMapper.v1
{
    public class StoreTypeProfile : Profile
    {
        public StoreTypeProfile()
        {
            CreateMap<StoreType, GetStoreTypeDto>();
            CreateMap<AddStoreTypeDto, StoreType>();
        }
    }
}
