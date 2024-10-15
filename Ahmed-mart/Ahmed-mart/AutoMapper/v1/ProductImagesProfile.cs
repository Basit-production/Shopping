using Ahmed_mart.Dtos.v1.ProductImagesDto;
using Ahmed_mart.Models.v1;
using AutoMapper;

namespace Ahmed_mart.AutoMapper.v1
{
    public class ProductImagesProfile : Profile
    {
        public ProductImagesProfile()
        {
            CreateMap<ProductImages, GetProductImagesDto>().ReverseMap();
            CreateMap<AddProductImagesDto, ProductImages>().ReverseMap();
        }
    }
}
