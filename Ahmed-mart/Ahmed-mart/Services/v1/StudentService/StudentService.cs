using Ahmed_mart.DbContexts.v1;
using Ahmed_mart.Dtos.v1.StudentDto;
using Ahmed_mart.Models.v1.MONGODB;
using Ahmed_mart.Repository.v1.UnitOfWork;
using Ahmed_mart.Repository.v1;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;

namespace Ahmed_mart.Services.v1.StudentService
{
    public class StudentService : BaseService, IStudentService
    {
        protected override string CacheKey => "studentCacheKey";
        private readonly MongoDbContext _mongoDbContext;

        public StudentService(
            IUnitOfWork unitOfWork,
            ILogger<BaseService> logger,
            IMapper mapper,
            IMemoryCache memoryCache,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            MongoDbContext mongoDbContext) :
            base(unitOfWork, logger, mapper, memoryCache, httpContextAccessor, configuration)
        {
            _mongoDbContext = mongoDbContext;
        }

        public async Task<ServiceResponse<IEnumerable<GetStudentDto>>> GetAllAsync()
        {
            var serviceResponse = new ServiceResponse<IEnumerable<GetStudentDto>>();
            try
            {
                if (_memoryCache.TryGetValue(CacheKey, out IEnumerable<GetStudentDto>? data))
                {
                    serviceResponse.Data = data;
                }
                else
                {
                    var result = await _mongoDbContext.Students.Find(x => true).ToListAsync();
                    data = _mapper.Map<IEnumerable<GetStudentDto>>(result);
                    data.ToList().ForEach(x =>
                    {
                        if (x.FileSize != null && double.TryParse(x.FileSize, out double fileSize))
                        {
                            x.FileSize = FormatFileSize(fileSize);
                        }
                        else
                        {
                            x.FileSize = "Unknown size.";
                        }
                    });

                    serviceResponse.Data = data;
                    _memoryCache.Set(CacheKey, data, GetCacheEntryOptions());
                }
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(GetAllAsync));
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetStudentDto>> GetByIdAsync(string id)
        {
            var serviceResponse = new ServiceResponse<GetStudentDto>();
            try
            {
                var result = await _mongoDbContext.Students.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (result != null)
                {
                    var data = _mapper.Map<GetStudentDto>(result);
                    if (data.FileSize != null && double.TryParse(data.FileSize, out double fileSize))
                    {
                        data.FileSize = FormatFileSize(fileSize);
                    }
                    else
                    {
                        data.FileSize = "Unknown size.";
                    }
                    serviceResponse.Data = data;
                }
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(GetByIdAsync));
            }
            return serviceResponse;
        }

        private string FormatFileSize(double fileSize)
        {
            const int kb = 1024;
            const int mb = 1024 * 1024;

            if (fileSize < kb)
            {
                return $"{fileSize} KB";
            }
            else if (fileSize < mb)
            {
                return $"{fileSize / kb} KB";
            }
            else
            {
                return $"{fileSize / mb} MB";
            }
        }

        public async Task<ServiceResponse<GetStudentDto>> AddAsync(AddStudentDto addStudentDto)
        {
            var serviceResponse = new ServiceResponse<GetStudentDto>();
            try
            {
                var objStudent = _mapper.Map<Student>(addStudentDto);
                if (addStudentDto.File != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        addStudentDto.File.CopyTo(memoryStream);
                        objStudent.FileSize = addStudentDto.File.Length / (1024.0 * 1024);
                        objStudent.ContentType = addStudentDto.File.ContentType;
                        objStudent.Content = memoryStream.ToArray();
                        objStudent.Base64File = Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
                await _mongoDbContext.Students.InsertOneAsync(objStudent);
                _memoryCache.Remove(CacheKey);
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(AddAsync));
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetStudentDto>> UpdateAsync(UpdateStudentDto updateStudentDto)
        {
            var serviceResponse = new ServiceResponse<GetStudentDto>();
            try
            {
                var objStudent = _mapper.Map<Student>(updateStudentDto);
                if (updateStudentDto.File != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        updateStudentDto.File.CopyTo(memoryStream);
                        objStudent.FileSize = updateStudentDto.File.Length / (1024.0 * 1024);
                        objStudent.ContentType = updateStudentDto.File.ContentType;
                        objStudent.Content = memoryStream.ToArray();
                        objStudent.Base64File = Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
                await _mongoDbContext.Students.ReplaceOneAsync(x => x.Id == objStudent.Id, objStudent);
                _memoryCache.Remove(CacheKey);
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(UpdateAsync));
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetStudentDto>> DeleteAsync(string id)
        {
            var serviceResponse = new ServiceResponse<GetStudentDto>();
            try
            {
                await _mongoDbContext.Students.DeleteOneAsync(x => x.Id == id);
                _memoryCache.Remove(CacheKey);
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(DeleteAsync));
            }
            return serviceResponse;
        }
    }
}
