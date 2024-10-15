using Ahmed_mart.Dtos.v1.OrderStatusDto;
using Ahmed_mart.Dtos.v1.StoreDto;
using Ahmed_mart.Models.v1;
using Ahmed_mart.Repository.v1;
using Ahmed_mart.Repository.v1.UnitOfWork;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace Ahmed_mart.Services.v1.OrdersStatusService
{
    public class OrdersStatusService : BaseService, IOrdersStatusService
    {
        protected override string CacheKey => "adminauthCacheKey";
        public OrdersStatusService(IUnitOfWork unitOfWork,
            ILogger<BaseService> logger,
            IMapper mapper,
            IMemoryCache memoryCache,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration) : base(unitOfWork, logger, mapper, memoryCache, httpContextAccessor, configuration)
        { }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User
            .FindFirstValue(ClaimTypes.NameIdentifier)!);

        private int GetUserTrackerId() => int.Parse(_httpContextAccessor.HttpContext!.User
            .FindFirstValue("UserTrackerID")!);

        public async Task<ServiceResponse<IEnumerable<GetOrderStatusDto>>> GetOrdersStatus()
        {
            var serviceResponse = new ServiceResponse<IEnumerable<GetOrderStatusDto>>();
            try
            {
                if (_memoryCache.TryGetValue(CacheKey, out IEnumerable<GetOrderStatusDto>? data))
                {
                    serviceResponse.Data = data;
                }
                else
                {
                    var _ordersStatusRepo = _unitOfWork.GetRepository<OrdersStatus>();
                    var result = await _ordersStatusRepo.Search(
                    x => x.IsDeleted == false);
                     data = _mapper.Map<IEnumerable<GetOrderStatusDto>>(result.OrderByDescending(x => x.CreatedAt));
                    serviceResponse.Data = data;
                    _memoryCache.Set(CacheKey, data, GetCacheEntryOptions());

                }
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(GetOrdersStatus));
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetOrderStatusDto>> GetOrderStatus(int id)
        {
            var serviceResponse = new ServiceResponse<GetOrderStatusDto>();
            try
            {
                var _ordersStatusRepo = _unitOfWork.GetRepository<OrdersStatus>();
                var result = await _ordersStatusRepo.GetByIdAsync(id);
                if (result != null)
                {
                    serviceResponse.Data = _mapper.Map<GetOrderStatusDto>(result);
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Order status not found.";
                }
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(GetOrderStatus));
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetOrderStatusDto>> AddOrderStatus(AddOrderStatusDto addOrderStatusDto)
        {
            var serviceResponse = new ServiceResponse<GetOrderStatusDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _ordersStatusRepo = _unitOfWork.GetRepository<OrdersStatus>();
                var data = _mapper.Map<OrdersStatus>(addOrderStatusDto);
                data.CreatedBy = 1;// GetUserId();
                data.CreatedAt = DateTime.Now;
                await _ordersStatusRepo.AddAsync(data);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                serviceResponse.Data = _mapper.Map<GetOrderStatusDto>(data);
                serviceResponse.Message = "Order status added successfully.";
                _memoryCache.Remove(CacheKey);
            }
            catch (Exception ex)
            {

                HandleException(serviceResponse, ex, nameof(AddOrderStatus));
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetOrderStatusDto>> UpdateOrderStatus(UpdateOrderStatusDto updateOrderStatusDto)
        {
            var serviceResponse = new ServiceResponse<GetOrderStatusDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _ordersStatusRepo= _unitOfWork.GetRepository<OrdersStatus>();
                var data = await _ordersStatusRepo.GetByIdAsync(updateOrderStatusDto.ID);
                if (data != null)
                {
                    data.Name = updateOrderStatusDto.Name;
                    data.Status = updateOrderStatusDto.Status;
                    data.ModifiedBy = 1;// GetUserId();
                    data.ModifiedAt = DateTime.Now;
                    await _ordersStatusRepo.UpdateAsync(data);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();
                    serviceResponse.Data = _mapper.Map<GetOrderStatusDto>(data);
                    serviceResponse.Message = "Order status updated successfully.";
                    _memoryCache.Remove(CacheKey);
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Order status not found.";
                }
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(UpdateOrderStatus));
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetOrderStatusDto>> DeleteOrderStatus(int id)
        {
            var serviceResponse = new ServiceResponse<GetOrderStatusDto>();
            try
            {
                var _ordersStatusRepo = _unitOfWork.GetRepository<OrdersStatus>();
                var data = await _ordersStatusRepo.GetByIdAsync(id);
                if (data != null)
                {
                    data.IsDeleted = true;
                    data.ModifiedBy = 1;//GetUserId();
                    data.ModifiedAt = DateTime.Now;
                    _ordersStatusRepo.Update(data);
                    await _unitOfWork.CommitAsync();
                    serviceResponse.Data = _mapper.Map<GetOrderStatusDto>(data);
                    serviceResponse.Message = "Order status deleted successfully.";
                    _memoryCache.Remove(CacheKey);
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Order status not found.";
                }
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(DeleteOrderStatus));
            }
            return serviceResponse;
        }
    }
}
