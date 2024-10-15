using Ahmed_mart.Dtos.v1.ProductDto;
using Ahmed_mart.Dtos.v1.ProductsDto;
using Ahmed_mart.Models.v1;
using Ahmed_mart.Services.v1.ProductsService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ahmed_mart.Controllers.v1
{
    //[Authorize]
    [ApiVersion("1.0")]
    public class ProductsController : BaseController
    {
        private readonly IProductsService _productsService;
        public ProductsController(IProductsService productsService)
        {
            _productsService = productsService;
        }
        [HttpGet("GetProducts")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetProducts()
        {
            var serviceResponse = await HandleServiceResponseAsync(_productsService.GetProducts());
            return serviceResponse;
        }
        [HttpGet("GetProduct/{Id:int}/{StoreId:int}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetProduct(int Id,int StoreId)
        {
            var serviceSerpose = await HandleServiceResponseAsync(_productsService.GetProduct(Id,StoreId));
            return serviceSerpose;
        }
        [HttpPost("AddProduct")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddProduct([FromForm] AddProductsDto addProductsDto)
        {
            var serviceSerpose = await HandleServiceResponseAsync(_productsService.AddProduct(addProductsDto));
            return serviceSerpose;
        }

        [HttpPut("UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateProduct([FromForm] UpdateProductsDto updateProductsDto)
        {
           var serviceSerpose = await HandleServiceResponseAsync(_productsService.UpdateProduct(updateProductsDto));
            return serviceSerpose;
        }
        [HttpDelete("DeleteProduct/{Id:int}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteProduct(int Id)
        {
            var serviceSerpose = await HandleServiceResponseAsync(_productsService.DeleteProduct(Id));
            return serviceSerpose;
        }
        [HttpPost("GetProductsOnSearch")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetProductsOnSearch([FromBody]SearchProductsDto searchProductsDto)
        {
            var serviceRerpose = await HandleServiceResponseAsync(_productsService.GetProductsOnSearch(searchProductsDto));
            return serviceRerpose;
        }
    }
}
