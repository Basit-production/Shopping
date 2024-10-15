using Ahmed_mart.Dtos.v1.StoreDto;
using Ahmed_mart.Dtos.v1.StoreTypeDto;
using Ahmed_mart.Models.v1;
using Ahmed_mart.Repository.v1;
using Ahmed_mart.Repository.v1.UnitOfWork;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;
using System.Security.Claims;

namespace Ahmed_mart.Services.v1.StoreTypeService
{
    public class StoreTypeService : BaseService, IStoreTypeService
    {
        protected override string CacheKey => "StoreTypeCacheKey";
        public StoreTypeService(
            IUnitOfWork unitOfWork,
            ILogger<BaseService> logger,
            IMapper mapper,
            IMemoryCache memoryCache,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration) :
            base(unitOfWork, logger, mapper, memoryCache, httpContextAccessor, configuration)
        {}

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User
            .FindFirstValue(ClaimTypes.NameIdentifier)!);
        string? GetUserRole() => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role); //return the role of current user

        public async Task<ServiceResponse<IEnumerable<GetStoreTypeDto>>> GetStoreTypes()
        {
            var serviceResponse = new ServiceResponse<IEnumerable<GetStoreTypeDto>>();
            try
            {
                if (_memoryCache.TryGetValue(CacheKey, out IEnumerable<GetStoreTypeDto>? data))
                {
                    serviceResponse.Data = data;
                }
                else
                {
                    var _storeType = _unitOfWork.GetRepository<StoreType>();
                    var result = await _storeType.SearchAsync(x => !x.IsDeleted);
                    data = _mapper.Map<IEnumerable<GetStoreTypeDto>>(result.OrderByDescending(x => x.CreatedAt));
                    serviceResponse.Data = data;
                    _memoryCache.Set(CacheKey, data, GetCacheEntryOptions());

                }
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(GetStoreTypes));
            }
            return serviceResponse;
        }
        public async Task<ServiceResponse<IEnumerable<GetStoreTypeDto>>> GetStoreTypesForStoreFront()
        {
            var serviceResponse = new ServiceResponse<IEnumerable<GetStoreTypeDto>>();
            try
            {
                var _storeTypeRepo = _unitOfWork.GetRepository<StoreType>();
                var result = await _storeTypeRepo.Search(x => x.IsDeleted == false);
                List<GetStoreTypeDto> data = new List<GetStoreTypeDto>();
                foreach (var type in result)
                {
                    var _storeRepo = _unitOfWork.GetRepository<Store>();
                    var stores = await _storeRepo.Search(x => x.StoreTypeID == type.ID);
                    if (stores.Count() > 0)
                    {
                        data.Add(_mapper.Map<GetStoreTypeDto>(type));
                    }
                }
                serviceResponse.Data = data;
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(GetStoreTypesForStoreFront));
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetStoreTypeDto>> AddStoreType(AddStoreTypeDto addstoreTypeDto)
        {
            var serviceResponse = new ServiceResponse<GetStoreTypeDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _storeTypeRepo = _unitOfWork.GetRepository<StoreType>();
                var obj = _mapper.Map<StoreType>(addstoreTypeDto);
                obj.CreatedBy = 1;//GetUserId();
                obj.CreatedAt = DateTime.Now;
                var data = await _storeTypeRepo.AddAsync(obj);
                await _unitOfWork.SaveChangesAsync();
                // Other tables inserts or updates
                await _unitOfWork.CommitAsync();
                serviceResponse.Data = _mapper.Map<GetStoreTypeDto>(data);
                // Clear cache after success
                _memoryCache.Remove(CacheKey);
            }
            catch (Exception ex)
            {
                await RollbackAndHandleException(serviceResponse, ex, nameof(AddStoreType));
            }
            return serviceResponse;
        }
       
        public async Task<ServiceResponse<GetStoreTypeDto>> UpdateStoreType(UpdateStoreTypeDto updatestoreTypeDto)
        {
            var serviceResponse=new ServiceResponse<GetStoreTypeDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _storeTypeRepo = _unitOfWork.GetRepository<StoreType>();
                var result = await _storeTypeRepo.GetByIdAsync(updatestoreTypeDto.ID);
                result.Name = updatestoreTypeDto.Name;
                result.Status = updatestoreTypeDto.Status;
                result.IsDeleted = updatestoreTypeDto.IsDeleted;
                result.ModifiedBy = 1;//GetUserId();
                result.ModifiedAt = DateTime.Now;
                var data = await _storeTypeRepo.UpdateAsync(result);
                await _unitOfWork.SaveChangesAsync();
                // Other tbls inserts or updates
                await _unitOfWork.CommitAsync();
                serviceResponse.Data = _mapper.Map<GetStoreTypeDto>(data);
                // Clear cache after success
                _memoryCache.Remove(CacheKey);
            }
            catch (Exception ex)
            {
                await RollbackAndHandleException(serviceResponse, ex, nameof(UpdateStoreType));
            }
            return serviceResponse;

        }
    }
}
