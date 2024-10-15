using Ahmed_mart.Helpers.v1;
using Ahmed_mart.Repository.v1;
using Ahmed_mart.Repository.v1.UnitOfWork;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace Ahmed_mart.Services.v1
{
    public abstract class BaseService
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ILogger<BaseService> _logger;
        protected readonly IMapper _mapper;
        protected readonly IMemoryCache _memoryCache;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IConfiguration _configuration;
        protected abstract string CacheKey { get; }

        protected BaseService(
            IUnitOfWork unitOfWork,
            ILogger<BaseService> logger,
            IMapper mapper,
            IMemoryCache memoryCache,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        protected async Task RollbackAndHandleException<T>(ServiceResponse<T> serviceResponse, Exception ex, string methodName)
        {
            await _unitOfWork.RollbackAsync();
            HandleException(serviceResponse, ex, methodName);
        }

        protected void HandleException<T>(ServiceResponse<T> serviceResponse, Exception ex, string methodName)
        {
            // Log the exception with stack trace
            _logger.LogError(ex, $"An unhandled exception occurred: {ex}");
            // Log inner exceptions with stack trace and type information
            foreach (var innerException in InnerExceptionsHelper.GetAllInnerExceptions(ex))
            {
                _logger.LogError(innerException, $"Inner Exception Type: {innerException.GetType().FullName}");
            }
            serviceResponse.Data = default;
            serviceResponse.Success = false;
            serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
            serviceResponse.Message = $"Error in {methodName}";
            serviceResponse.Errors = new List<string> { ex.Message };
        }

        protected MemoryCacheEntryOptions GetCacheEntryOptions()
        {
            return new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(1),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                Priority = CacheItemPriority.Normal
            };
        }
    }
}
