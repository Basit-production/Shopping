using Ahmed_mart.Dtos.v1.RelatedProductsDto;
using Ahmed_mart.Models.v1;
using AutoMapper;
namespace Ahmed_mart.AutoMapper.v1
{
    public class RelatedProductsProfile : Profile
    {
        public RelatedProductsProfile()
        {
            CreateMap<RelatedProducts, GetRelatedProductsDto>().ReverseMap();
            CreateMap<AddRelatedProductsDto, RelatedProducts>().ReverseMap();
        }
    }
}
