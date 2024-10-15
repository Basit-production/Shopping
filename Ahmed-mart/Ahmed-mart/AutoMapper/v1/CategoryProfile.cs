using Ahmed_mart.Dtos.v1.CategoryDtos;
using Ahmed_mart.Models.v1;
using AutoMapper;

namespace Ahmed_mart.AutoMapper.v1
{
    public class CategoryProfile: Profile
    {
        public CategoryProfile() 
        {
            CreateMap<Category, GetCategoryDto>().ReverseMap();
            CreateMap<AddCategoryDto, Category>().ReverseMap();
            CreateMap<Category, DeleteCategoryDto>().ReverseMap();
        }
             
    }
}
