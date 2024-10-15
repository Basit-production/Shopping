using Ahmed_mart.Dtos.v1.CategoryDtos;
using Ahmed_mart.Repository.v1;
using Ahmed_mart.Repository.v1.UnitOfWork;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Text;
using System.IO;
using System.Reflection;
using Ahmed_mart.Models.v1;
using Ahmed_mart.Dtos.v1.AdminDtos;
using Ahmed_mart.Dtos.v1.FileDtos;
using Ahmed_mart.Services.v1.FileService;
using Microsoft.Extensions.Caching.Distributed;

namespace Ahmed_mart.Services.v1.CategoryService
{
    public class CategoryService : BaseService ,ICategoryService
    {
        protected override string CacheKey => "CategoryCacheKey";
        private readonly IFileService _fileService;
        public CategoryService(IUnitOfWork unitOfWork,
            ILogger<BaseService> logger,
            IFileService fileService,
            IMapper mapper,
            IMemoryCache memoryCache,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration) :
            base(unitOfWork, logger, mapper, memoryCache, httpContextAccessor, configuration
            )
        { 
            _fileService = fileService;
        }
        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User
           .FindFirstValue(ClaimTypes.NameIdentifier)!);
        string? GetUserRole() =>  _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Role); //return the role of current user

        public async Task<ServiceResponse<IEnumerable<GetCategoryDto>>>GetCategories()
        {
            var serviceResponse= new ServiceResponse<IEnumerable<GetCategoryDto>>();
            try
            {
                if(_memoryCache.TryGetValue(CacheKey,out IEnumerable<GetCategoryDto>? data))
                {
                    serviceResponse.Data = data;
                }
                else
                {
                    var categories = _unitOfWork.GetRepository<Category>();
                    var result = await categories.SearchAsync(x=>!x.IsDeleted);
                    data = _mapper.Map<IEnumerable<GetCategoryDto>>(result.OrderByDescending(x => x.CreatedAt));
                    serviceResponse.Data = data;
                    _memoryCache.Set(CacheKey, data, GetCacheEntryOptions());
                }

            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(GetCategories));
            }
            return serviceResponse;
        }
        public async Task<ServiceResponse<GetCategoryDto>> GetSingleCategory(int Id)
        {
            var serviceResponse=new ServiceResponse<GetCategoryDto>();
            try
            {
                var category = _unitOfWork.GetRepository<Category>();
                var result = await category.GetByIdAsync(Id);
                var data = _mapper.Map<GetCategoryDto>(result);
                serviceResponse.Data = data;
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(GetCategories));
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCategoryDto>> AddCategory(AddCategoryDto addCategoryDto)
        {
            var serviceResponse = new ServiceResponse<GetCategoryDto>();
            try
            {
                using var transcation = _unitOfWork.BeginTransactionAsync();
                var category = _unitOfWork.GetRepository<Category>();
                var mapCategory = _mapper.Map<Category>(addCategoryDto);
                mapCategory.CreatedBy = 1;//GetUserId();
                mapCategory.CreatedAt = DateTime.Now;
                if (addCategoryDto.AutoDiscount == true)
                {
                    mapCategory.Discount= addCategoryDto.Discount;
                }
                else
                {
                    mapCategory.Discount = 0;
                }
                var data = await category.AddAsync(mapCategory);
                await _unitOfWork.SaveChangesAsync();
                // Other tables inserts or updates
                if(addCategoryDto.File !=null && addCategoryDto.File.Length > 0)
                {
                    var objUploadFileDto = new UploadFileDto()
                    {
                        Directory = $"Categories/CategoryImage/{data.ID}",
                        File = addCategoryDto.File
                    };
                    var dataFileService = await _fileService.UploadFileAsync(objUploadFileDto);
                    if (dataFileService.Success)
                    {
                        data.Path = dataFileService.Data?.Path;
                        await category.UpdateAsync(data);
                        await _unitOfWork.SaveChangesAsync();
                    }
                }
                await _unitOfWork.CommitAsync();
                serviceResponse.Data = _mapper.Map<GetCategoryDto>(data);
                // Clear cache after success
                _memoryCache.Remove(CacheKey);
            }
            catch(Exception ex)
            {
                await RollbackAndHandleException(serviceResponse, ex, nameof(AddCategory));
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCategoryDto>> UpdateCategory(UpdateCategoryDto updateCategoryDto)
        {
            var serviceResponse = new ServiceResponse<GetCategoryDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var category = _unitOfWork.GetRepository<Category>();
                var result = await category.GetByIdAsync(updateCategoryDto.ID);
                result.CategoryCode = updateCategoryDto.CategoryCode;
                result.CategoryName = updateCategoryDto.CategoryName;
                result.AutoDiscount = updateCategoryDto.AutoDiscount;
                if (updateCategoryDto.AutoDiscount == true)
                {
                    result.Discount = updateCategoryDto.Discount;
                }
                else
                {
                    result.Discount = 0;
                }
                result.Status = updateCategoryDto.Status;
                result.ModifiedBy = 1;//GetUserId();
                result.ModifiedAt = DateTime.Now;
                result.IsDeleted = updateCategoryDto.IsDeleted;
                var data = await category.UpdateAsync(result);
                await _unitOfWork.SaveChangesAsync();
                // Other tbls inserts or updates
                if (updateCategoryDto.File != null && updateCategoryDto.File.Length > 0)
                {
                    var objUploadFileDto = new UploadFileDto()
                    {
                        Directory = $"Categories/CategoryImage/{data.ID}",
                        File = updateCategoryDto.File
                    };
                    var dataFileService = await _fileService.UploadFileAsync(objUploadFileDto);
                    if (dataFileService.Success)
                    {
                        data.Path = dataFileService.Data?.Path;
                        await category.UpdateAsync(data);
                        await _unitOfWork.SaveChangesAsync();
                    }
                }
                await _unitOfWork.CommitAsync();
                serviceResponse.Data = _mapper.Map<GetCategoryDto>(data);
                // Clear cache after success
                _memoryCache.Remove(CacheKey);
            }
            catch (Exception ex)
            {
                await RollbackAndHandleException(serviceResponse, ex, nameof(GetCategoryDto));
            }
            return serviceResponse;
        }
        public async Task<ServiceResponse<DeleteCategoryDto>> DeleteCategory(int Id)
        {
            var serviceResponse = new ServiceResponse<DeleteCategoryDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var category = _unitOfWork.GetRepository<Category>();
                var result = await category.GetByIdAsync(Id);
                result.IsDeleted = true;
                result.ModifiedBy = 1;//GetUserId();
                result.ModifiedAt = DateTime.UtcNow;
                var data = await category.UpdateAsync(result);
                await _unitOfWork.SaveChangesAsync();

                // Other tbls inserts or updates

                await _unitOfWork.CommitAsync();

                serviceResponse.Data = _mapper.Map<DeleteCategoryDto>(data);

                // Clear cache after success
                _memoryCache.Remove(CacheKey);
            }
            catch (Exception ex)
            {
                await RollbackAndHandleException(serviceResponse, ex, nameof(DeleteCategory));
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<IEnumerable<GetCategoryDto>>> GetCategoriesOnSearch(CategoryOnSearchDto categoryOnSearchDto)
        {
            var serviceResponse = new ServiceResponse<IEnumerable<GetCategoryDto>>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var category = _unitOfWork.GetRepository<Category>();
                var result = await category.Search(
                        x => x.CategoryCode.ToLower().Contains(categoryOnSearchDto.CategoryCode) ||
                        x.CategoryName.ToLower().Contains(categoryOnSearchDto.CategoryName) &&
                        x.IsDeleted == false &&
                        x.Status == true 
                        //x.StoreId == addProductCategoryDto.StoreId
                        );
                var data = _mapper.Map<IEnumerable<GetCategoryDto>>(result.OrderByDescending(x => x.CreatedAt));
                serviceResponse.Data = data;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.GetType().ToString();
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(GetCategoriesOnSearch)}");
            }
            return serviceResponse;
        }
    }
}
