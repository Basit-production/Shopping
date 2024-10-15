using Ahmed_mart.Dtos.v1.ProductOptionDetailsDto;
using Ahmed_mart.Models.v1;
using AutoMapper;
namespace Ahmed_mart.AutoMapper.v1
{
    public class ProductOptionDetailsProfile : Profile
    {
        public ProductOptionDetailsProfile()
        {
            CreateMap<ProductOptionDetails, GetProductOptionDetailsDto>().ReverseMap();
            CreateMap<AddProductOptionDetailsDto, ProductOptionDetails>().ReverseMap();
        }
    }
}
    

