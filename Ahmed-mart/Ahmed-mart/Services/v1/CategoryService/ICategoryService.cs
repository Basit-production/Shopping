using Ahmed_mart.Dtos.v1.CategoryDtos;
using Ahmed_mart.Repository.v1;

namespace Ahmed_mart.Services.v1.CategoryService
{
    public interface ICategoryService
    {
        Task<ServiceResponse<IEnumerable<GetCategoryDto>>> GetCategories();
        Task<ServiceResponse<GetCategoryDto>> GetSingleCategory(int Id);
        //Task<ServiceResponse<GetCategoryDto>> GetProductCategory(int id);
        Task<ServiceResponse<GetCategoryDto>> AddCategory(AddCategoryDto addCategoryDto);
        Task<ServiceResponse<GetCategoryDto>> UpdateCategory(UpdateCategoryDto updateCategoryDto);
        Task<ServiceResponse<DeleteCategoryDto>> DeleteCategory(int Id);
        Task<ServiceResponse<IEnumerable<GetCategoryDto>>> GetCategoriesOnSearch(CategoryOnSearchDto categoryOnSearchDto);
        //Task<ServiceResponse<GetCategoryDto>> UpdateProductCategory(UpdateCategoryDto updateProductCategoryDto);
        //Task<ServiceResponse<GetCategoryDto>> DeleteProductCategory(int id);
        //Task<ServiceResponse<IEnumerable<GetCategoryDto>>> GetProductCategoriesByStore(int storeId);
        //Task<ServiceResponse<IEnumerable<GetCategoryDto>>> GetProductCategoriesOnSearch(AddCategoryDto addProductCategoryDto);
    }
}
