using Ahmed_mart.Dtos.v1.GetOrderByStoreDtos;
using Ahmed_mart.Dtos.v1.OrdersDto;
using Ahmed_mart.Dtos.v1.ReportsDto;
using Ahmed_mart.Repository.v1;

namespace Ahmed_mart.Services.v1.OrdersService
{
    public interface IOrdersService 
    {
        Task<ServiceResponse<IEnumerable<GetOrdersDto>>> GetOrders();
        Task<ServiceResponse<GetOrdersDto>> GetOrder(int Id);
       // Task<ServiceResponse<GetOrdersDto>> AddOrder(AddOrdersDto addOrdersDto);
        Task<ServiceResponse<GetOrdersDto>> UpdateOrder(UpdateOrdersDto updateOrdersDto);
        Task<ServiceResponse<GetOrdersDto>> DeleteOrder(int Id);
        Task<ServiceResponse<IEnumerable<GetOrdersDto>>> GetOrdersByStore(GetOrderByStoreFilterDto addOrdersDto);
        Task<ServiceResponse<IEnumerable<GetOrdersDto>>> GetRecentBuyersByStore(int storeId);
        Task<ServiceResponse<IList<GetCustomerSalesReportDto>>> GetCustomerSalesReportByStore(GetOrderByStoreFilterDto addOrdersDto);
        Task<ServiceResponse<IList<GetSoldProductsReportDto>>> GetSoldProductsReportByStore(GetOrderByStoreFilterDto addOrdersDto);
        Task<ServiceResponse<IList<GetSalesReportDto>>> GetSalesReportByStore(GetOrderByStoreFilterDto addOrdersDto);
        Task<ServiceResponse<IList<GetBestSellingProductsDto>>> GetBestSellingProductsByStore(AddOrdersDto addOrdersDto);
        Task<ServiceResponse<IEnumerable<GetOrdersDto>>> GetTotalOrdersByStore(int storeId);
        Task<ServiceResponse<IList<GetSoldProductsReportDto>>> GetSoldProductsByStore(int storeId);
       // Task<ServiceResponse<IEnumerable<GetOrdersDto>>> GetOrdersByCustomerAndStore(int storeId, int customersId);
    }
}
