using Ahmed_mart.Dtos.v1.DirectoryDto;
using Ahmed_mart.Repository.v1;

namespace Ahmed_mart.Services.v1.DirectoryService
{
    public interface IDirectoryService
    {
        Task<ServiceResponse<GetDirectoryDto>> ManageDirectoryCopyAndOverwiteFilesAsync(int storeId);
    }
}
