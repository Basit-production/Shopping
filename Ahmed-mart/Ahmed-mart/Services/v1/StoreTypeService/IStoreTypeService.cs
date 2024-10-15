using Ahmed_mart.Dtos.v1.StoreTypeDto;
using Ahmed_mart.Repository.v1;

namespace Ahmed_mart.Services.v1.StoreTypeService
{
    public interface IStoreTypeService
    {
        Task<ServiceResponse<IEnumerable<GetStoreTypeDto>>> GetStoreTypes();
        Task<ServiceResponse<IEnumerable<GetStoreTypeDto>>> GetStoreTypesForStoreFront();
        Task<ServiceResponse<GetStoreTypeDto>> AddStoreType(AddStoreTypeDto addstoreTypeDto);
        Task<ServiceResponse<GetStoreTypeDto>> UpdateStoreType(UpdateStoreTypeDto updatestoreTypeDto);
    }
}
