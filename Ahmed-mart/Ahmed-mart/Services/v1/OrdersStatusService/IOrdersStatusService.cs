using Ahmed_mart.Dtos.v1.OrderStatusDto;
using Ahmed_mart.Repository.v1;

namespace Ahmed_mart.Services.v1.OrdersStatusService
{
    public interface IOrdersStatusService
    {
        Task<ServiceResponse<IEnumerable<GetOrderStatusDto>>> GetOrdersStatus();
        Task<ServiceResponse<GetOrderStatusDto>> GetOrderStatus(int id);
        Task<ServiceResponse<GetOrderStatusDto>> AddOrderStatus(AddOrderStatusDto addOrderStatusDto);
        Task<ServiceResponse<GetOrderStatusDto>> UpdateOrderStatus(UpdateOrderStatusDto updateOrderStatusDto);
        Task<ServiceResponse<GetOrderStatusDto>> DeleteOrderStatus(int id);
    }
}
