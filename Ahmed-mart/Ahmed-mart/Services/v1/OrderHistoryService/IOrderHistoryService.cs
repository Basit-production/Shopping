using Ahmed_mart.Dtos.v1.OrderHistoryDto;
using Ahmed_mart.Repository.v1;

namespace Ahmed_mart.Services.v1.OrderHistoryService
{
    public interface IOrderHistoryService
    {
        //Task<ServiceResponse<GetOrderHistoryDto>> GetOrderHistory();
        Task<ServiceResponse<GetOrderHistoryDto>> AddOrderHistory(AddOrderHistoryDto addOrderHistoryDto);
        Task<ServiceResponse<GetOrderHistoryDto>> UpdateOrderHistory(UpdateOrderHistoryDto updateOrderHistoryDto);
    }
}
