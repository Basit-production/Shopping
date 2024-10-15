using Ahmed_mart.Dtos.v1.ProductOptionsDto;
using Ahmed_mart.Models.v1;
using AutoMapper;


namespace Ahmed_mart.AutoMapper.v1
{
    public class ProductOptionsProfile : Profile
    {
        public ProductOptionsProfile()
        {
            CreateMap<ProductOptions, GetProductOptionsDto>().ReverseMap();
            CreateMap<AddProductOptionsDto, ProductOptions>().ReverseMap();
        }
    }
}
