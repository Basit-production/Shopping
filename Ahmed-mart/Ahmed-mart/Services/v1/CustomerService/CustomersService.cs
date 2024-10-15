using Ahmed_mart.Dtos.v1.CustomersDto;
using Ahmed_mart.Dtos.v1.FileDtos;
using Ahmed_mart.Models.v1;
using Ahmed_mart.Repository.v1;
using Ahmed_mart.Repository.v1.UnitOfWork;
using Ahmed_mart.Services.v1.FileService;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;

namespace Ahmed_mart.Services.v1.CustomerService
{
    public class CustomersService : BaseService , ICustomersService
    {
        protected override string CacheKey => "customerauthCacheKey";
        private readonly IFileService _fileService;
        public CustomersService(
            IFileService fileService,
            IUnitOfWork unitOfWork,
            ILogger<BaseService> logger,
            IMapper mapper,
            IMemoryCache memoryCache,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration) : base(unitOfWork, logger, mapper, memoryCache, httpContextAccessor, configuration)
        {
            _fileService = fileService;
        }
        public async Task<ServiceResponse<IEnumerable<GetCustomersDto>>> GetCustomers()
        {
            var serviceResponse = new ServiceResponse<IEnumerable<GetCustomersDto>>();
            try
            {
                if(_memoryCache.TryGetValue(CacheKey,out IEnumerable<GetCustomersDto>? data))
                {
                    serviceResponse.Data = data;
                }
                else
                {
                    var _customerRepo = _unitOfWork.GetRepository<Customers>();
                    var customers =await _customerRepo.SearchAsync(x => !x.IsDeleted, x => x.Store, x => x.CustomerAddresses);
                    data = _mapper.Map<IEnumerable<GetCustomersDto>>(customers.OrderByDescending(x=>x.CreatedAt));
                    serviceResponse.Data = data;
                    _memoryCache.Set(CacheKey, data, GetCacheEntryOptions());
                }
                
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(GetCustomers));
               
            }
            return serviceResponse;
        }
        public async Task<ServiceResponse<GetCustomersDto>> AddCustomer(AddCustomersDto addCustomersDto)
        {
            var serviceResponse = new ServiceResponse<GetCustomersDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _customerRepo = _unitOfWork.GetRepository<Customers>();
                var customerData = _mapper.Map<Customers>(addCustomersDto);
                if (await CustomerExists(customerData.ID, customerData.Email))
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"Customer already exist.";
                    return serviceResponse;
                }
                customerData.CreatedAt = DateTime.UtcNow;
                var data = await _customerRepo.AddAsync(customerData);
                await _unitOfWork.SaveChangesAsync();

                //add CustomerUser
                var _customerUserRepo = _unitOfWork.GetRepository<CustomerUsers>();
                PasswordHashSaltUtility(addCustomersDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
                var customerUser = new CustomerUsers()
                {
                    Name = data.Name,
                    Email = data.Email,
                    MobileNumber = data.MobileNumber,
                    StoreID = data.StoreID,
                    CustomersID = data.ID,
                    Status = true,
                    CreatedAt = DateTime.Now,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    CreatedBy = data.ID
                };
                await _customerUserRepo.AddAsync(customerUser);
                await _unitOfWork.SaveChangesAsync();
                //
                if (addCustomersDto.File != null && addCustomersDto.File.Length > 0)
                {
                    var objUploadFileDto = new UploadFileDto()
                    {
                        Directory = $"Customers/CustomerImage/{customerData.ID}",
                        File = addCustomersDto.File
                    };
                    var dataFileService = await _fileService.UploadFileAsync(objUploadFileDto);
                    if (dataFileService.Success)
                    {
                        data.Path = dataFileService.Data.Path;
                        data.CreatedBy = data.ID;
                        await _customerRepo.UpdateAsync(customerData);
                        await _unitOfWork.SaveChangesAsync();
                    }
                }
                if (addCustomersDto.CustomerAddresses != null && addCustomersDto.CustomerAddresses.Count > 0)
                {
                    var _addressRepo = _unitOfWork.GetRepository<CustomerAddresses>();
                    foreach (var Address in addCustomersDto.CustomerAddresses.ToList())
                    {
                        var newAddress = _mapper.Map<CustomerAddresses>(Address);
                        newAddress.CustomersID = data.ID;
                        newAddress.CreatedAt = DateTime.UtcNow;
                        newAddress.CreatedBy = 1;//GetUserId();
                        await _addressRepo.AddAsync(newAddress);
                        await _unitOfWork.SaveChangesAsync();
                    }
                }

                await _unitOfWork.CommitAsync();
                serviceResponse.Data = _mapper.Map<GetCustomersDto>(customerData);
                serviceResponse.Message = $"Customer added successfully.";
                _memoryCache.Remove(CacheKey);

            }
            catch (Exception ex)
            {
                await RollbackAndHandleException(serviceResponse, ex, nameof(AddCustomer));
            }
            return serviceResponse;
        }
        public async Task<ServiceResponse<GetCustomersDto>> UpdateCustomer(UpdateCustomersDto updateCustomersDto)
        {
            var serviceResponse = new ServiceResponse<GetCustomersDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _customerRepo = _unitOfWork.GetRepository<Customers>();
                var result = await _customerRepo.GetSingleAsync(updateCustomersDto.ID);
                if (result == null)
                {
                    result.Name = updateCustomersDto.Name;
                    if (!string.IsNullOrEmpty(updateCustomersDto.Email))
                    {
                        if (await CustomerExists(result.ID, updateCustomersDto.Email))
                        {
                            serviceResponse.Success = false;
                            serviceResponse.Message = $"Customer already exist.";
                            return serviceResponse;
                        }
                        else
                        {
                            result.Email = updateCustomersDto.Email;
                        }
                    }
                    result.MobileNumber = updateCustomersDto.MobileNumber;
                    //if (!string.IsNullOrEmpty(updateCustomersDto.Password))
                    //{
                    //    PasswordHashSaltUtility(updateCustomersDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
                    //    result.PasswordHash = passwordHash;
                    //    result.PasswordSalt = passwordSalt;
                    //}
                    result.Status = updateCustomersDto.Status;
                    if (updateCustomersDto.File != null && updateCustomersDto.File.Length > 0)
                    {
                        var objUploadFileDto = new UploadFileDto()
                        {
                            Directory = $"Customers/CustomerImage/{result.ID}",
                            File = updateCustomersDto.File
                        };
                        var dataFileService = await _fileService.UploadFileAsync(objUploadFileDto);
                        if (dataFileService.Success)
                        {
                            result.Path = dataFileService.Data.Path;
                        }
                    }
                    result.StoreID = updateCustomersDto.StoreID;
                    result.CustomerGroupID = updateCustomersDto.CustomerGroupID;
                    //result.Organization = updateCustomersDto.Organization;
                    result.ModifiedBy = result.ID;//GetUserId();//Because customer only Responsible from Create And Update Account
                    result.ModifiedAt = DateTime.Now;
                    var data = await _customerRepo.UpdateAsync(result);
                    await _unitOfWork.SaveChangesAsync();
                    //Update CustomerUser
                    var _customerUserRepo = _unitOfWork.GetRepository<CustomerUsers>();
                    PasswordHashSaltUtility(updateCustomersDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
                    var customerUser = new CustomerUsers()
                    {
                        Name = data.Name,
                        Email = data.Email,
                        MobileNumber = data.MobileNumber,
                        StoreID = data.StoreID,
                        CustomersID = data.ID,
                        Status = true,
                        ModifiedAt = DateTime.Now,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        ModifiedBy = data.ID
                    };
                    await _customerUserRepo.UpdateAsync(customerUser);
                    await _unitOfWork.SaveChangesAsync();
                    //
                    if (updateCustomersDto.CustomerAddresses != null && updateCustomersDto.CustomerAddresses.Count > 0)
                    {
                        var _addressRepo = _unitOfWork.GetRepository<CustomerAddresses>();
                        foreach (var Address in updateCustomersDto.CustomerAddresses.ToList())
                        {
                            if (Address.Id == 0)
                            {
                                var newAddress = _mapper.Map<CustomerAddresses>(Address);
                                newAddress.CustomersID = data.ID;
                                newAddress.CreatedAt = DateTime.UtcNow;
                                newAddress.CreatedBy = data.ID;//GetUserId();
                                await _addressRepo.AddAsync(newAddress);
                                await _unitOfWork.SaveChangesAsync();
                            }
                            else
                            {
                                var ExistingAddress = updateCustomersDto.CustomerAddresses.FirstOrDefault(x => x.Id == Address.Id);
                                if (ExistingAddress != null)
                                {
                                    var UpdateAddress = await _addressRepo.GetByIdAsync(ExistingAddress.Id);
                                    UpdateAddress.CustomersID = updateCustomersDto.ID;
                                    UpdateAddress.Name = Address.Name;
                                    UpdateAddress.MobileNumber = Address.MobileNumber;
                                    UpdateAddress.PINCode = Address.PINCode;
                                    UpdateAddress.AddressLine1 = Address.AddressLine1;
                                    UpdateAddress.AddressLine2 = Address.AddressLine2;
                                    UpdateAddress.LandMark = Address.LandMark;
                                    UpdateAddress.City = Address.City;
                                    UpdateAddress.StateID = Address.StateID;
                                    UpdateAddress.AddressType = Address.AddressType;
                                    UpdateAddress.IsDefault = Address.IsDefault;
                                    UpdateAddress.Status = Address.Status;
                                    UpdateAddress.ModifiedBy = updateCustomersDto.ID;
                                    UpdateAddress.ModifiedAt = DateTime.Now;
                                    await _addressRepo.UpdateAsync(UpdateAddress);
                                    await _unitOfWork.SaveChangesAsync();
                                }
                            }
                        }
                    }

                }
                await _unitOfWork.CommitAsync();
                serviceResponse.Data = _mapper.Map<GetCustomersDto>(result);
                serviceResponse.Message = $"Customer update successfully.";
                _memoryCache.Remove(CacheKey);
            }
            catch (Exception ex)
            {
                await RollbackAndHandleException(serviceResponse, ex, nameof(UpdateCustomer));
            }
            return serviceResponse;
        }
        private async Task<bool> CustomerExists(int id, string mail)
        {
            var _CustomerRepo = _unitOfWork.GetRepository<Customers>();
            var data = await _CustomerRepo.SearchAsync(x => x.ID == id && x.Email.ToLower() == mail.ToLower());
            bool Exists = data.Any();
            if (Exists)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void PasswordHashSaltUtility(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
