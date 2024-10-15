using Ahmed_mart.Dtos.v1.ProductDto;
using Ahmed_mart.Dtos.v1.ProductsDto;
using Ahmed_mart.Models.v1;
using AutoMapper;


namespace Ahmed_mart.AutoMapper.v1
{
    public class ProductsProfile : Profile
    {
        public ProductsProfile()
        {
            CreateMap<Products, GetProductsDto>().ReverseMap();
            CreateMap<AddProductsDto, Products>().ReverseMap();
            CreateMap<Products, DeleteProductsDto>().ReverseMap();
        }
    }
}
