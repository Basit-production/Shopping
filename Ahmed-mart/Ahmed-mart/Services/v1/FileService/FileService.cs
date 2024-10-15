
using Ahmed_mart.Dtos.v1.FileDtos;
using Ahmed_mart.Repository.v1;
using Ahmed_mart.Repository.v1.UnitOfWork;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;

namespace Ahmed_mart.Services.v1.FileService
{
    public class FileService : BaseService, IFileService
    {
        protected override string CacheKey => "fileCacheKey";
        public FileService(
            IUnitOfWork unitOfWork,
            ILogger<BaseService> logger,
            IMapper mapper,
            IMemoryCache memoryCache,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration) :
            base(unitOfWork, logger, mapper, memoryCache, httpContextAccessor, configuration)
            { }

        public async Task<ServiceResponse<GetFileDto>> UploadFileAsync(UploadFileDto uploadFileDto)
        {
            var serviceResponse = new ServiceResponse<GetFileDto>();
            try
            {
                if (uploadFileDto.File != null && uploadFileDto.File.Length != 0)
                {
                    var rootPath = _configuration.GetSection("Web:Path").Value;
                    var directory = $"Documents/{uploadFileDto.Directory}/";
                    var basePath = Path.Combine(rootPath, directory);
                    if (!Directory.Exists(basePath))
                    {
                        Directory.CreateDirectory(basePath);
                    }
                    var fileName = $"{Guid.NewGuid()}.{uploadFileDto.File.FileName.Split('.')[1]}";
                    var path = Path.Combine(basePath, fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        await uploadFileDto.File.CopyToAsync(fileStream);
                    }
                    var data = new GetFileDto
                    {
                        Path = Path.Combine(directory, fileName)
                    };
                    serviceResponse.Data = data;
                    serviceResponse.Message = $"File uploaded successfully.";
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"File not found.";
                }
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.GetType().ToString();
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(UploadFileAsync)}");
            }
            return serviceResponse;
        }
    }
}
