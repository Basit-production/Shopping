using Ahmed_mart.Dtos.v1.OrderHistoryDto;
using Ahmed_mart.Models.v1;
using Ahmed_mart.Repository.v1;
using Ahmed_mart.Repository.v1.UnitOfWork;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
namespace Ahmed_mart.Services.v1.OrderHistoryService
{
    public class OrderHistoryService :BaseService, IOrderHistoryService
    {
        protected override string CacheKey => "adminauthCacheKey";

        public OrderHistoryService(IUnitOfWork unitOfWork,
            ILogger<BaseService> logger,
            IMapper mapper,
            IMemoryCache memoryCache,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration) :base (unitOfWork, logger, mapper, memoryCache, httpContextAccessor, configuration)
        { }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User
            .FindFirstValue(ClaimTypes.NameIdentifier)!);

        private int GetUserTrackerId() => int.Parse(_httpContextAccessor.HttpContext!.User
            .FindFirstValue("UserTrackerID")!);


        public async Task<ServiceResponse<GetOrderHistoryDto>> AddOrderHistory(AddOrderHistoryDto addOrderHistoryDto)
        {
            var serviceResponse = new ServiceResponse<GetOrderHistoryDto>();
            try
            {

                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _orderHistoryRepo = _unitOfWork.GetRepository<OrderHistory>();
                var data = _mapper.Map<OrderHistory>(addOrderHistoryDto);
                data.CreatedBy = 1;//GetUserId();
                data.CreatedAt = DateTime.Now;

                var _ordersRepo = _unitOfWork.GetRepository<Orders>();
                var orderData = await _ordersRepo.GetByIdAsync(addOrderHistoryDto.OrdersID);
                if (orderData != null)
                {
                    orderData.OrdersStatusID = addOrderHistoryDto.OrdersStatusID;
                    orderData.ModifiedBy = 1;// GetUserId();
                    orderData.ModifiedAt = DateTime.Now;
                    await _ordersRepo.UpdateAsync(orderData);
                    await _unitOfWork.SaveChangesAsync();
                }

                await _orderHistoryRepo.AddAsync(data);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                _memoryCache.Remove(CacheKey);

                serviceResponse.Data = _mapper.Map<GetOrderHistoryDto>(data);
                serviceResponse.Message = "Order History added successfully.";
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(AddOrderHistory));
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetOrderHistoryDto>> UpdateOrderHistory(UpdateOrderHistoryDto updateOrderHistoryDto)
        {
            var serviceResponse = new ServiceResponse<GetOrderHistoryDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _orderHistoryRepo = _unitOfWork.GetRepository<OrderHistory>();
                var data = await _orderHistoryRepo.GetByIdAsync(updateOrderHistoryDto.ID);
                if (data != null)
                {
                    data.Comments = updateOrderHistoryDto.Comments;
                    data.OrdersStatusID = updateOrderHistoryDto.OrdersStatusID;
                    data.ModifiedBy = 1;// GetUserId();
                    data.ModifiedAt = DateTime.Now;
                    await _orderHistoryRepo.UpdateAsync(data);
                    await _unitOfWork.SaveChangesAsync();

                    var _ordersRepo = _unitOfWork.GetRepository<Orders>();
                    var orderData = await _ordersRepo.GetByIdAsync(updateOrderHistoryDto.OrdersID);
                    if (orderData != null)
                    {
                        orderData.OrdersStatusID = updateOrderHistoryDto.OrdersStatusID;
                        orderData.ModifiedBy = 1;// GetUserId();
                        orderData.ModifiedAt = DateTime.Now;
                        await _ordersRepo.UpdateAsync(orderData);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    await _unitOfWork.CommitAsync();
                    
                }
                await _unitOfWork.CommitAsync();
                serviceResponse.Data = _mapper.Map<GetOrderHistoryDto>(data);
                serviceResponse.Message = "Order History added successfully.";
                _memoryCache.Remove(CacheKey);
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(UpdateOrderHistory));
              
            }
            return serviceResponse;
        }

    }
}
