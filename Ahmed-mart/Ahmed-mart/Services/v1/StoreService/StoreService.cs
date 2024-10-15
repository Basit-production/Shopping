using Ahmed_mart.Dtos.v1.FileDtos;
using Ahmed_mart.Dtos.v1.StoreDto;
using Ahmed_mart.Models.v1;
using Ahmed_mart.Repository.v1;
using Ahmed_mart.Repository.v1.UnitOfWork;
using Ahmed_mart.Services.v1.FileService;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace Ahmed_mart.Services.v1.StoreService
{
    public class StoreService : BaseService, IStoreService
    {
        protected override string CacheKey => "StoreCacheKey";
        private readonly IFileService _fileService;

        public StoreService(
            IFileService fileService,
            IUnitOfWork unitOfWork,
            ILogger<BaseService> logger,
            IMapper mapper,
            IMemoryCache memoryCache,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration) :
            base(unitOfWork, logger, mapper, memoryCache, httpContextAccessor, configuration)
        {
            _fileService = fileService;
        }
        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User
            .FindFirstValue(ClaimTypes.NameIdentifier)!);
        string? GetUserRole() => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role); //return the role of current user
        public async Task<ServiceResponse<IEnumerable<GetStoreDto>>> GetStores()
        {
            var serviceResponse=new ServiceResponse<IEnumerable<GetStoreDto>>();
            try
            {
                if(_memoryCache.TryGetValue(CacheKey,out IEnumerable<GetStoreDto>? data))
                {
                    serviceResponse.Data = data;
                }
                else
                {
                    var _store = _unitOfWork.GetRepository<Store>();
                    var result = await _store.SearchAsync(x=> !x.IsDeleted);
                    data =_mapper.Map<IEnumerable<GetStoreDto>>(result.OrderByDescending(x => x.CreatedAt));
                    serviceResponse.Data = data;
                    _memoryCache.Set(CacheKey, data, GetCacheEntryOptions());

                }
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(GetStores));
            }
            return serviceResponse;
        }
        public async Task<ServiceResponse<GetStoreDto>> GetStore(int Id)
        {
            var serviceResponse = new ServiceResponse<GetStoreDto>();
            try
            {
                var _store = _unitOfWork.GetRepository<Store>();
                var result = await _store.GetSingleAsync(Id);
                var data = _mapper.Map<GetStoreDto>(result);
                if (data.PaymentPreference == true)
                {
                    var _paymentGatewayConfigurationRepo = _unitOfWork.GetRepository<PaymentGatewayConfiguration>();
                    var payment = await _paymentGatewayConfigurationRepo.Search(x => x.StoreID == result.ID);
                    if (payment != null)
                    {
                        var pay = payment.FirstOrDefault();
                        data.Key = pay.Key;
                        data.Secret = pay.Secret;
                    }
                }
                serviceResponse.Data = data;
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(GetStore));
            }
            return serviceResponse;
        }
        
        public async Task<ServiceResponse<GetStoreDto>> AddStore(AddStoreDto addStoreDto)
        {
            var serviceResponse = new ServiceResponse<GetStoreDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _storeRepo = _unitOfWork.GetRepository<Store>();
                var Storedata = _mapper.Map<Store>(addStoreDto);
                Storedata.CreatedBy = 1;// GetUserId();
                Storedata.CreatedAt = DateTime.Now;
                await _storeRepo.AddAsync(Storedata);
                await _unitOfWork.SaveChangesAsync();
                if (addStoreDto.File != null && addStoreDto.File.Length > 0)
                {
                    var objUploadFileDto = new UploadFileDto()
                    {
                        Directory = $"Stores/StoreLogo/{Storedata.ID}",
                        File = addStoreDto.File
                    };
                    var dataFileService = await _fileService.UploadFileAsync(objUploadFileDto);
                    if (dataFileService.Success)
                    {
                        Storedata.Path = dataFileService.Data.Path;
                        _storeRepo.Update(Storedata);
                        await _unitOfWork.SaveChangesAsync();
                    }
                }
                //Directory service
               
                // store name
                string trimmedStoreName = string.Concat(Storedata.Name.Where(c => !char.IsWhiteSpace(c)));
                bool IsStoreCode = false;
                // check store code
                var stores = await _storeRepo.GetAllAsync();
                foreach (var s in stores)
                {
                    if (s.Code == trimmedStoreName)
                    {
                        IsStoreCode = true;
                    }
                }
                // update store code
                if (IsStoreCode == true)
                {
                    Storedata.Code = trimmedStoreName + Storedata.ID;
                }
                else
                {
                    Storedata.Code = trimmedStoreName;
                }
                Storedata.ModifiedAt = DateTime.Now;
                Storedata.ModifiedBy = Storedata.ID;
                _storeRepo.Update(Storedata);
                await _unitOfWork.SaveChangesAsync();
                //
                if (Storedata.IsPayment == true)
                {
                    if (Storedata.PaymentPreference == true)
                    {
                        // add razorpay configuration data
                        var _paymentGatewayConfigurationRepo = _unitOfWork.GetRepository<PaymentGatewayConfiguration>();
                        var razorpay = new PaymentGatewayConfiguration()
                        {
                            Key = addStoreDto.Key,
                            Secret = addStoreDto.Secret,
                            StoreID = Storedata.ID,
                            Status = true,
                            CreatedAt = DateTime.Now,
                            CreatedBy =1// GetUserId()
                        };
                       await _paymentGatewayConfigurationRepo.AddAsync(razorpay);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        var _paymentGatewayConfigurationRepo = _unitOfWork.GetRepository<PaymentGatewayConfiguration>();
                        var paymentInfo = await _paymentGatewayConfigurationRepo.Search(x => x.StoreID == null);
                        var objInfo = paymentInfo.FirstOrDefault();
                        var razorpay = new PaymentGatewayConfiguration()
                        {
                            Key = objInfo.Key,
                            Secret = objInfo.Secret,
                            StoreID = Storedata.ID,
                            Status = true,
                            CreatedAt = DateTime.Now,
                            CreatedBy = 1 //GetUserId()
                        };
                        await _paymentGatewayConfigurationRepo.AddAsync(razorpay);
                        await _unitOfWork.SaveChangesAsync();
                    }
                }
                await _unitOfWork.CommitAsync();
                serviceResponse.Data = _mapper.Map<GetStoreDto>(Storedata);
                serviceResponse.Message = "Store added successfully.";
                // Clear cache after success
                _memoryCache.Remove(CacheKey);
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(GetStore));
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetStoreDto>> UpdateStore(UpdateStoreDto updateStoreDto)
        {
            var serviceResponse = new ServiceResponse<GetStoreDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _storeRepo = _unitOfWork.GetRepository<Store>();
                var data = await _storeRepo.GetByIdAsync(updateStoreDto.ID);
                if (data != null)
                {

                    data.Name = updateStoreDto.Name;
                    data.Description = updateStoreDto.Description == null ? "" : updateStoreDto.Description;
                    data.Status = updateStoreDto.Status;
                    data.GSTIN = updateStoreDto.GSTIN;
                    data.StoreContact = updateStoreDto.StoreContact;
                    data.IsPayment = updateStoreDto.IsPayment;
                    data.NumberPreference = updateStoreDto.NumberPreference;
                    data.PaymentPreference = updateStoreDto.PaymentPreference;
                    //data.StoreType = updateStoreDto.StoreTypeId;
                    data.IsFreeBirdModule = updateStoreDto.IsFreeBirdModule;
                    data.ModifiedBy = 1;// GetUserId();
                    data.ModifiedAt = DateTime.Now;
                    
                    // upload document
                    if (updateStoreDto.File != null && updateStoreDto.File.Length > 0)
                    {
                        var objUploadFileDto = new UploadFileDto()
                        {
                            Directory = $"Stores/StoreLogo/{data.ID}",
                            File = updateStoreDto.File
                        };
                        var dataFileService = await _fileService.UploadFileAsync(objUploadFileDto);
                        if (dataFileService.Success)
                        {
                            data.Path = dataFileService.Data.Path;
                        }
                    }

                    // add/update prefixes
                    if (updateStoreDto.NumberPreference == true && updateStoreDto.PrefixDetails.Count > 0)
                    {
                        foreach (var pd in updateStoreDto.PrefixDetails)
                        {
                            if (pd.ID == 0)
                            {
                                var _prefixesRepo = _unitOfWork.GetRepository<Prefixes>();
                                var addPrefix = new Prefixes()
                                {
                                    StoreID = data.ID,
                                    TransactionType = pd.TransactionType,
                                    TransactionLength = pd.TransactionLength,
                                    Prefix = pd.Prefix,
                                    StartNumber = pd.StartNumber,
                                    CurrentNumber = pd.CurrentNumber,
                                    ModifiedBy = 1, //GetUserId(),
                                    ModifiedAt = DateTime.Now,

                                };
                                await _prefixesRepo.AddAsync(addPrefix);
                                await _unitOfWork.SaveChangesAsync();
                            }
                            else
                            {
                                var _prefixesRepo = _unitOfWork.GetRepository<Prefixes>();
                                var prefix = await _prefixesRepo.GetByIdAsync(pd.ID);
                                prefix.TransactionType = pd.TransactionType;
                                prefix.TransactionLength = pd.TransactionLength;
                                prefix.Prefix = pd.Prefix;
                                prefix.StartNumber = pd.StartNumber;
                                prefix.CurrentNumber = pd.CurrentNumber;
                                prefix.ModifiedBy = 1;// GetUserId();
                                prefix.ModifiedAt = DateTime.Now;
                                await _prefixesRepo.UpdateAsync(prefix);
                                await _unitOfWork.SaveChangesAsync();
                            }
                        }
                    }
                    await _storeRepo.UpdateAsync(data);
                    await _unitOfWork.SaveChangesAsync();

                    if (data.IsPayment == true)
                    {
                        var _paymentGatewayConfigurationRepo = _unitOfWork.GetRepository<PaymentGatewayConfiguration>();
                        var paymentInfo = await _paymentGatewayConfigurationRepo.Search(x => x.StoreID == data.ID);
                        if (paymentInfo.Any())
                        {
                            var pay = paymentInfo.FirstOrDefault();

                            if (data.PaymentPreference == true)
                            {
                                pay.Key = updateStoreDto.Key;
                                pay.Secret = updateStoreDto.Secret;
                                pay.Status = true;
                                pay.IsDeleted = false;
                                pay.ModifiedBy = 1;// GetUserId();
                                pay.ModifiedAt = DateTime.Now;
                               await _paymentGatewayConfigurationRepo.UpdateAsync(pay);
                                await _unitOfWork.SaveChangesAsync();
                            }
                            else
                            {
                                var adminPaymentInfo = await _paymentGatewayConfigurationRepo.Search(x => x.StoreID == null);
                                var objInfo = adminPaymentInfo.FirstOrDefault();
                                pay.Key = updateStoreDto.Key;
                                pay.Secret = updateStoreDto.Secret;
                                pay.Status = true;
                                pay.IsDeleted = false;
                                pay.ModifiedBy = 1;// GetUserId();
                                pay.ModifiedAt = DateTime.Now;
                               await  _paymentGatewayConfigurationRepo.UpdateAsync(pay);
                                await _unitOfWork.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            if (data.PaymentPreference == true)
                            {
                                // add razorpay configuration data
                                var razorpay = new PaymentGatewayConfiguration()
                                {
                                    Key = updateStoreDto.Key,
                                    Secret = updateStoreDto.Secret,
                                    StoreID = data.ID,
                                    Status = true,
                                    CreatedAt = DateTime.Now,
                                    CreatedBy = 1// GetUserId()
                                };
                               await _paymentGatewayConfigurationRepo.AddAsync(razorpay);
                            }
                            else
                            {
                                var adminPaymentInfo = await _paymentGatewayConfigurationRepo.Search(x => x.StoreID == null);
                                var objInfo = adminPaymentInfo.FirstOrDefault();
                                var razorpay = new PaymentGatewayConfiguration()
                                {
                                    Key = objInfo.Key,
                                    Secret = objInfo.Secret,
                                    StoreID = data.ID,
                                    Status = true,
                                    CreatedAt = DateTime.Now,
                                    CreatedBy =1// GetUserId()
                                };
                               await _paymentGatewayConfigurationRepo.AddAsync(razorpay);
                                await _unitOfWork.SaveChangesAsync();
                            }
                        }
                    }
                    else
                    {
                        var _paymentGatewayConfigurationRepo = _unitOfWork.GetRepository<PaymentGatewayConfiguration>();
                        var paymentInfo = await _paymentGatewayConfigurationRepo.Search(x => x.StoreID == data.ID);
                        if (paymentInfo.Any())
                        {
                            var pay = paymentInfo.FirstOrDefault();
                            pay.Status = false;
                            pay.IsDeleted = true;
                            pay.ModifiedBy = 1;// GetUserId();
                            pay.ModifiedAt = DateTime.Now;
                           await  _paymentGatewayConfigurationRepo.UpdateAsync(pay);
                            await _unitOfWork.SaveChangesAsync();
                        }
                    }
                    await _unitOfWork.CommitAsync();
                    serviceResponse.Data = _mapper.Map<GetStoreDto>(data);
                    serviceResponse.Message = "Store updated successfully.";
                    // Clear cache after success
                    _memoryCache.Remove(CacheKey);
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Store not found.";
                }
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(UpdateStore));
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetStoreDto>> DeleteStore(int id)
        {
            var serviceResponse = new ServiceResponse<GetStoreDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _storeRepo = _unitOfWork.GetRepository<Store>();
                var data = await _storeRepo.GetByIdAsync(id);
                if (data != null)
                {
                    data.IsDeleted = true;
                    data.ModifiedBy = 1;// GetUserId();
                    data.ModifiedAt = DateTime.Now;
                    await _storeRepo.UpdateAsync(data);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();
                    serviceResponse.Data = _mapper.Map<GetStoreDto>(data);
                    serviceResponse.Message = "Store deleted successfully.";
                    // Clear cache after success
                    _memoryCache.Remove(CacheKey);
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Store not found.";
                }
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(DeleteStore));
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<IEnumerable<GetStoreDto>>> GetUserStores(int userId)
        {
            var serviceResponse = new ServiceResponse<IEnumerable<GetStoreDto>>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _storeRepo = _unitOfWork.GetRepository<Store>();
                var result = await _storeRepo.Search(x => x.UserID == userId);
                var data = result;
                if (data != null)
                {
                    serviceResponse.Data = _mapper.Map<IEnumerable<GetStoreDto>>(data);
                    serviceResponse.Message = "User logged in successfully.";
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Store not found.";
                }
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(GetUserStores));
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetStoreDto>> GetStoreByCode(string code)
        {
            var serviceResponse = new ServiceResponse<GetStoreDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _storeRepo = _unitOfWork.GetRepository<Store>();
                var result = await _storeRepo.Search(x => x.Code.ToLower() == code.ToLower(), x => x.User);
                var data = result.FirstOrDefault();
                if (data != null)
                {
                    var mapData = _mapper.Map<GetStoreDto>(data);
                    serviceResponse.Data = mapData;
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Data not found.";
                }
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(GetStoreByCode));
            }
            return serviceResponse;
        }
    }
}
