using Ahmed_mart.Dtos.v1.FileDtos;
using Ahmed_mart.Dtos.v1.RoleDtos;
using Ahmed_mart.Repository.v1;

namespace Ahmed_mart.Services.v1.FileService
{
    public interface IFileService
    {
        Task<ServiceResponse<GetFileDto>> UploadFileAsync(UploadFileDto uploadFileDto);
    }
}
