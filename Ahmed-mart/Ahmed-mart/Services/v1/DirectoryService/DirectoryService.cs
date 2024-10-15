using Ahmed_mart.Dtos.v1.DirectoryDto;
using Ahmed_mart.Models.v1;
using Ahmed_mart.Repository.v1;
using Ahmed_mart.Repository.v1.UnitOfWork;
using Ahmed_mart.Services.v1.FileService;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;

namespace Ahmed_mart.Services.v1.DirectoryService
{
    public class DirectoryService : BaseService,IDirectoryService
    {
        protected override string CacheKey => "DirectoryCacheKey";

        public DirectoryService(
            IUnitOfWork unitOfWork,
            ILogger<BaseService> logger,
            IFileService fileService,
            IMapper mapper,
            IMemoryCache memoryCache,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration) :
             base(unitOfWork, logger, mapper, memoryCache, httpContextAccessor, configuration)
        {
           
        }

        public async Task<ServiceResponse<GetDirectoryDto>> ManageDirectoryCopyAndOverwiteFilesAsync(int storeId)
        {
            var serviceResponse = new ServiceResponse<GetDirectoryDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _storeRepo = _unitOfWork.GetRepository<Store>();
                var storeData = await _storeRepo.GetByIdAsync(storeId);
                if (storeData != null)
                {
                    var rootPath = _configuration.GetSection("Web:Path").Value;
                    var sourcePath = Path.Combine(rootPath, $"Stores/StoreFront/{storeData.Template}/");
                    string trimmedStoreName = string.Concat(storeData.Name.Where(c => !char.IsWhiteSpace(c)));
                    var directory = $"Stores/{trimmedStoreName}/";
                    var destinationPath = Path.Combine(rootPath, directory);

                    if (Directory.Exists(sourcePath))
                    {
                        if (!Directory.Exists(destinationPath))
                        {
                            Directory.CreateDirectory(destinationPath);
                        }

                        string fileName = string.Empty;
                        string[] sourceFiles = Directory.GetFiles(sourcePath);
                        foreach (string sourceFile in sourceFiles)
                        {
                            fileName = sourceFile.Substring(sourcePath.Length);
                            File.Copy(Path.Combine(sourcePath, fileName), Path.Combine(destinationPath, fileName), true);
                        }

                        var data = new GetDirectoryDto
                        {
                            //Path = Path.Combine(directory, fileName)
                            Path = directory
                        };

                        serviceResponse.Data = data;
                        serviceResponse.Message = $"Directory created & files copied successfully.";
                    }
                    else
                    {
                        serviceResponse.Success = false;
                        serviceResponse.Message = $"Source path doesn't exist.";
                    }
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"Store not found.";
                }
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.GetType().ToString();
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(ManageDirectoryCopyAndOverwiteFilesAsync)}");
            }
            return serviceResponse;
        }
    }
}
