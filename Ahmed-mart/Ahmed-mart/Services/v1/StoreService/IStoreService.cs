
using Ahmed_mart.Dtos.v1.SmtpDetailsDto;
using Ahmed_mart.Dtos.v1.StoreDto;
using Ahmed_mart.Repository.v1;

namespace Ahmed_mart.Services.v1.StoreService
{
    public interface IStoreService
    {
        Task<ServiceResponse<IEnumerable<GetStoreDto>>> GetStores();
        Task<ServiceResponse<GetStoreDto>> GetStore(int Id);
        Task<ServiceResponse<GetStoreDto>> AddStore(AddStoreDto addStoreDto);
        Task<ServiceResponse<GetStoreDto>> UpdateStore(UpdateStoreDto updateStoreDto);
        Task<ServiceResponse<GetStoreDto>> DeleteStore(int Id);
        Task<ServiceResponse<IEnumerable<GetStoreDto>>> GetUserStores(int UserId);
        Task<ServiceResponse<GetStoreDto>> GetStoreByCode(string code);

        //Task<ServiceResponse<GetStoreDto>> GetStoreFront(int id);
        //Task<ServiceResponse<IEnumerable<GetStoreDto>>> GetVendorStores(int vendorId);
        //Task<ServiceResponse<IEnumerable<GetStoreDto>>> GetUserStores(int UserId);
        //Task<ServiceResponse<GetStoreDto>> GetStoreByCode(string code);
        //Task<ServiceResponse<GetSmtpDetailsDto>> GetSmtpDetails(int storeId);
        //Task<ServiceResponse<GetSmtpDetailsDto>> AddSmtpDetails(AddSmtpDetailsDto addSmtpDetailsDto);
        //Task<ServiceResponse<GetSmtpDetailsDto>> UpdateSmtpDetails(UpdateSmtpDetailsDto updateSmtpDetailsDto);
    }
}
