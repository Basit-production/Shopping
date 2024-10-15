using Ahmed_mart.Dtos.v1.ProductAttributesDto;
using Ahmed_mart.Models.v1;
using AutoMapper;
namespace Ahmed_mart.AutoMapper.v1
{
    public class ProductAttributesProfile : Profile
    {
        public ProductAttributesProfile()
        {
            CreateMap<ProductAttributes, GetProductAttributesDto>().ReverseMap();
            CreateMap<AddProductAttributesDto, ProductAttributes>().ReverseMap();
        }
    }
}