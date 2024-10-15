using Ahmed_mart.Dtos.v1.CategoryDtos;
using Ahmed_mart.Services.v1.CategoryService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ahmed_mart.Controllers.v1
{
    //[Authorize]
    [ApiVersion("1.0")]
    
    public class CategoryController : BaseController
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("GetCategories")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task <IActionResult> GetCategories()
        {
            var serviceResponse = await HandleServiceResponseAsync(_categoryService.GetCategories());
            return serviceResponse;
        }
        [HttpGet("GetSingleCategory/{Id:int}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSingleCategory(int Id)
        {
            var serviceResponse = await HandleServiceResponseAsync(_categoryService.GetSingleCategory(Id));
            return serviceResponse;
        }
        [HttpPost("AddCategory")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddCategory([FromForm]AddCategoryDto addCategoryDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_categoryService.AddCategory(addCategoryDto));
            return serviceResponse;
        }
        [HttpPost("UpdateCategory")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateCategory([FromForm] UpdateCategoryDto updateCategoryDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_categoryService.UpdateCategory(updateCategoryDto));
            return serviceResponse;
        }
        [HttpDelete("DeleteCategory/{Id:int}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteCategory(int Id)
        {
            var serviceResponse = await HandleServiceResponseAsync(_categoryService.DeleteCategory(Id));
            return serviceResponse;
        }

        [HttpPost("GetCategoriesOnSearch")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCategoriesOnSearch([FromBody] CategoryOnSearchDto categoryOnSearchDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_categoryService.GetCategoriesOnSearch(categoryOnSearchDto));
            return serviceResponse;
        }
    }
}
