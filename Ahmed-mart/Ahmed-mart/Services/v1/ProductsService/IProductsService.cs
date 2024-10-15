using Ahmed_mart.Dtos.v1.ProductDto;
using Ahmed_mart.Dtos.v1.ProductsDto;
using Ahmed_mart.Repository.v1;
using System.Threading.Tasks;

namespace Ahmed_mart.Services.v1.ProductsService
{
    public interface IProductsService
    {
        Task<ServiceResponse<IEnumerable<GetProductsDto>>> GetProducts();
        Task<ServiceResponse<GetProductsDto>> GetProduct(int Id ,int StoreId);
        Task<ServiceResponse<GetProductsDto>> AddProduct(AddProductsDto addProductsDto);
        Task<ServiceResponse<GetProductsDto>> UpdateProduct(UpdateProductsDto updateProductsDto);
        Task<ServiceResponse<DeleteProductsDto>> DeleteProduct(int id);
        Task<ServiceResponse<IList<GetProductsDto>>> GetProductsOnSearch(SearchProductsDto searchProductsDto);
    }
}
