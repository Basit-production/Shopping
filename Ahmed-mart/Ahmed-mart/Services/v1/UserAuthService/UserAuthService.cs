using Ahmed_mart.Dtos.v1.AdminDtos;
using Ahmed_mart.Dtos.v1.EmailDto;
using Ahmed_mart.Dtos.v1.FileDtos;
using Ahmed_mart.Dtos.v1.LoginDtos;
using Ahmed_mart.Dtos.v1.OtpDto;
using Ahmed_mart.Dtos.v1.SmsDtos;
using Ahmed_mart.Dtos.v1.UserDtos;
using Ahmed_mart.Models.v1;
using Ahmed_mart.Repository.v1;
using Ahmed_mart.Repository.v1.GenericRepository;
using Ahmed_mart.Repository.v1.UnitOfWork;
using Ahmed_mart.Services.v1.EmailService;
using Ahmed_mart.Services.v1.FileService;
using Ahmed_mart.Services.v1.SmsService;
using AutoMapper;
using IdentityModel.OidcClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using YamlDotNet.Core;

namespace Ahmed_mart.Services.v1.UserAuthService
{
    public class UserAuthService : BaseService, IUserAuthService
    {
        protected override string CacheKey => "userauthCacheKey"; 
        private readonly IFileService _fileService;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        private readonly IGenericRepository<Otp> _otpRepo;

        public UserAuthService(
            IFileService fileService,
            IGenericRepository<Otp> otpRepo,
            IEmailService emailService,
            ISmsService smsService,
            IUnitOfWork unitOfWork,
            ILogger<BaseService> logger,
            IMapper mapper,
            IMemoryCache memoryCache,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration) :
            base(unitOfWork, logger, mapper, memoryCache, httpContextAccessor, configuration)
        {
            _fileService = fileService;
            _emailService = emailService;
            _smsService = smsService;
            _otpRepo = otpRepo;
        }
        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User
           .FindFirstValue(ClaimTypes.NameIdentifier)!);

        private int GetUserTrackerId() => int.Parse(_httpContextAccessor.HttpContext!.User
            .FindFirstValue("UserTrackerID")!);
        private string GetUserRole() => _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);

        //Start
        public async Task<ServiceResponse<GetUserDto>> Login(GetLoginDto loginDto)
        {
            var serviceResponse = new ServiceResponse<GetUserDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _userRepo = _unitOfWork.GetRepository<User>();
                var result = await _userRepo.GetFirstOrDefaultAsync(
                    x => x.Email.ToLower().Equals(loginDto.Email.ToLower()),
                    x => x.Role);
                var data = _mapper.Map<GetUserDto>(result);
                if (data == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"User not found.";
                }
                else if (!VerifyPasswordHashSalt(loginDto.Password, result))
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"Wrong password.";
                }
                else if (data.IsLocked)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"User is locked.";
                }
                else if (data.IsDeleted)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"User is deleted.";
                }
                else if (data.Status == false)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"User is inactive.";
                }
                else
                {
                    var _userTrackerRepo = _unitOfWork.GetRepository<UserTracker>();
                    var userTracker = await _userTrackerRepo.GetFirstOrDefaultAsync(x => x.UserID == result.ID && x.Status == true);
                    if (userTracker != null)
                    {
                        userTracker.Status = false;
                        userTracker.ModifiedBy = result.ID;
                        userTracker.ModifiedAt = DateTime.Now;
                        await _userTrackerRepo.UpdateAsync(userTracker);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    var objUserTracker = new UserTracker()
                    {
                        UserID = result.ID,
                        LogIn = DateTime.Now.TimeOfDay,
                        CreatedBy = result.ID,
                        CreatedAt = DateTime.Now
                    };
                    await _userTrackerRepo.AddAsync(objUserTracker);
                    await _unitOfWork.SaveChangesAsync();
                    result.Token = CreateToken(result, objUserTracker);
                    //added
                    var refreshToken = GenerateRefreshToken();
                    SetRefreshToken(result, refreshToken);
                    //stop
                    await _userRepo.UpdateAsync(result);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();

                    data.Token = result.Token;
                    serviceResponse.Data = data;
                    serviceResponse.Message = $"User logged in successfully.";
                }
            }
            catch (Exception ex)
            {
                await RollbackAndHandleException(serviceResponse, ex, nameof(Login));
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetUserDto>> AddUser(AddUserDto addUserDto)
        {
            var serviceResponse = new ServiceResponse<GetUserDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _UserRepo = _unitOfWork.GetRepository<User>();
                var Userdata = _mapper.Map<User>(addUserDto);
                if (await UserExists(Userdata.ID, Userdata.Email))
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"User already exist.";
                    return serviceResponse;
                }
                PasswordHashSaltUtility(addUserDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
                Userdata.PasswordHash = passwordHash;
                Userdata.PasswordSalt = passwordSalt;
                Userdata.RoleID = addUserDto.RoleID;
                Userdata.CreatedAt = DateTime.Now;
                var data = await _UserRepo.AddAsync(Userdata);
                await _unitOfWork.SaveChangesAsync();
                data.CreatedBy = Userdata.ID;
                await _UserRepo.UpdateAsync(Userdata);
                await _unitOfWork.SaveChangesAsync();
                if (addUserDto.File != null && addUserDto.File.Length > 0)
                {
                    var objUploadFileDto = new UploadFileDto()
                    {
                        Directory = $"User/UserImage/{Userdata.ID}",
                        File = addUserDto.File
                    };
                    var dataFileService = await _fileService.UploadFileAsync(objUploadFileDto);
                    if (dataFileService.Success)
                    {
                        data.Path = dataFileService.Data.Path;
                        await _UserRepo.UpdateAsync(Userdata);
                        await _unitOfWork.SaveChangesAsync();
                    }
                }
                //if (addUserDto.Store != null)
                //{
                //    //
                //    addUserDto.Store.StoreDelivery.PaymentInfo = addUserDto.Store.StoreDelivery.PaymentInfo == null ? "" : addUserDto.Store.StoreDelivery.PaymentInfo;
                //    addUserDto.Store.StoreDelivery.AdditionalDeliveryInfo = addUserDto.Store.StoreDelivery.AdditionalDeliveryInfo == null ? "" : addUserDto.Store.StoreDelivery.AdditionalDeliveryInfo;
                //    addUserDto.Store.StoreDelivery.StoreAddress = addUserDto.Store.StoreDelivery.StoreAddress == null ? "" : addUserDto.Store.StoreDelivery.StoreAddress;
                //    addUserDto.Store.StoreDelivery.PickUpAddress = addUserDto.Store.StoreDelivery.PickUpAddress == null ? "" : addUserDto.Store.StoreDelivery.PickUpAddress;
                //    var store = _mapper.Map<Store>(addUserDto.Store);
                //    store.CreatedBy = data.Id;
                //    store.CreatedAt = DateTime.Now;
                //    store.UserId = data.Id;
                //    await _storeRepo.AddAsync(store);
                //    await _unitOfWork.CommitAsync();

                //    //Directory service
                //    var dataDirectoryService = await _directoryService.ManageDirectoryCopyAndOverwiteFilesAsync(store.Id);
                //    store.TemplatePath = dataDirectoryService.Data.Path;

                //    // store name
                //    string trimmedStoreName = string.Concat(store.Name.Where(c => !char.IsWhiteSpace(c)));
                //    bool IsStoreCode = false;
                //    // check store code
                //    var stores = await _storeRepo.GetAllAsync();
                //    foreach (var s in stores)
                //    {
                //        if (s.Code == trimmedStoreName)
                //        {
                //            IsStoreCode = true;
                //        }
                //    }

                //    // update store code
                //    if (IsStoreCode == true)
                //    {
                //        store.Code = trimmedStoreName + store.Id;
                //    }
                //    else
                //    {
                //        store.Code = trimmedStoreName;
                //    }

                //    store.ModifiedAt = DateTime.Now;
                //    store.ModifiedBy = data.Id;
                //    _storeRepo.Update(store);
                //    await _unitOfWork.CommitAsync();
                //}
                await _unitOfWork.CommitAsync();
                serviceResponse.Data = _mapper.Map<GetUserDto>(Userdata);
                serviceResponse.Message = $"User added successfully.";
            }
            catch (Exception ex)
            {
                await RollbackAndHandleException(serviceResponse, ex, nameof(AddUser));
            }
            return serviceResponse;
        }
        private void PasswordHashSaltUtility(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHashSalt(string password, User user)
        {
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                {
                    return false;
                }
            }
            return true;
        }

        private string CreateToken(User user, UserTracker objUserTracker)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim("UserTrackerId", objUserTracker.ID.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.Name)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(descriptor);

            return tokenHandler.WriteToken(token);
        }
        //refresh token
        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.UtcNow.AddDays(1),
                Created = DateTime.UtcNow
            };
            return refreshToken;
        }
        private void SetRefreshToken(User user, RefreshToken refreshToken)
        {
            var httpResponse = _httpContextAccessor.HttpContext!.Response;
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.Expires
            };
            httpResponse.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
            user.RefreshToken = refreshToken.Token;
            user.RefreshTokenCreatedAt = refreshToken.Created;
            user.RefreshTokenTokenExpiresAt = refreshToken.Expires;
        }

        public async Task<ServiceResponse<GetUserDto>> UpdateUser(UpdateUserDto updateUserDto)
        {
            var serviceResponse = new ServiceResponse<GetUserDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _User = _unitOfWork.GetRepository<User>();
                var result = await _User.GetByIdAsync(updateUserDto.ID);
                if (result != null)
                {
                    result.FirstName = updateUserDto.FirstName;
                    result.LastName = updateUserDto.LastName;
                    if (!string.IsNullOrEmpty(updateUserDto.Email))
                    {
                        if (await UserExists(result.ID, updateUserDto.Email))
                        {
                            serviceResponse.Success = false;
                            serviceResponse.Message = $"{GetUserRole()} already exist.";
                            return serviceResponse;
                        }
                        else
                        {
                            result.Email = updateUserDto.Email;
                        }
                    }
                    result.MobileNumber = updateUserDto.MobileNumber;
                    if (!string.IsNullOrEmpty(updateUserDto.Password))
                    {
                        PasswordHashSaltUtility(updateUserDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
                        result.PasswordHash = passwordHash;
                        result.PasswordSalt = passwordSalt;
                    }
                    result.Status = updateUserDto.Status;
                    if (updateUserDto.File != null && updateUserDto.File.Length > 0)
                    {
                        var objUploadFileDto = new UploadFileDto()
                        {
                            Directory = $"User/UserImage/{result.ID}",
                            File = updateUserDto.File
                        };
                        var dataFileService = await _fileService.UploadFileAsync(objUploadFileDto);
                        if (dataFileService.Success)
                        {
                            result.Path = dataFileService.Data.Path;
                        }
                    }
                    // result.ModifiedBy = GetUserId();
                    result.ModifiedBy = 1;
                    result.ModifiedAt = DateTime.Now;
                    var data = await _User.UpdateAsync(result);
                    await _unitOfWork.SaveChangesAsync();
                    // Other tbls inserts or updates
                    await _unitOfWork.CommitAsync();
                    serviceResponse.Data = _mapper.Map<GetUserDto>(data);
                    serviceResponse.Message = $"{GetUserRole()} updated successfully.";
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"{GetUserRole()} not found.";
                }
            }
            catch (Exception ex)
            {
                await RollbackAndHandleException(serviceResponse, ex, nameof(UpdateUser));
            }
            return serviceResponse;
        }

        public async Task<bool> UserExists(int Id, string email)
        {
            var _userRepo = _unitOfWork.GetRepository<User>();
            var data = await _userRepo.Search(
                x => x.ID != Id &&
                x.Email.ToLower() == email.ToLower());
            bool Exist = data.Any();
            if (Exist)
            {
                return true;
            }
            return false;
        }

        public async Task<ServiceResponse<GetUserDto>> LockUnlockUser(int Id)
        {
            var serviceResponse = new ServiceResponse<GetUserDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _User = _unitOfWork.GetRepository<User>();
                var result = await _User.GetByIdAsync(Id);
                if (result != null)
                {
                    result.IsLocked = !result.IsLocked;
                    // result.ModifiedBy = GetUserId();
                    result.ModifiedBy = 1;
                    result.ModifiedAt = DateTime.Now;
                    var Userdata = await _User.UpdateAsync(result);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();
                    serviceResponse.Data = _mapper.Map<GetUserDto>(Userdata);
                    serviceResponse.Message = $"{GetUserRole()} {(Userdata.IsLocked ? "locked" : "unlocked")} successfully.";
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"{GetUserRole()} not found.";
                }
            }
            catch (Exception ex)
            {
                await RollbackAndHandleException(serviceResponse, ex, nameof(LockUnlockUser));
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetUserDto>> UpdateUserStatus(int Id)
        {
            var serviceResponse = new ServiceResponse<GetUserDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _User = _unitOfWork.GetRepository<User>();
                var result = await _User.GetByIdAsync(Id);
                if (result != null)
                {
                    result.Status = !result.Status;
                    //result.ModifiedBy = GetUserId();
                    result.ModifiedBy = 1;
                    result.ModifiedAt = DateTime.Now;
                    await _User.UpdateAsync(result);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();
                    serviceResponse.Data = _mapper.Map<GetUserDto>(result);
                    serviceResponse.Message = $"{GetUserRole()} {(result.Status ? "Activated" : "De-Activated")} successfully.";
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"{GetUserRole()} not found.";
                }
            }
            catch (Exception ex)
            {
                await RollbackAndHandleException(serviceResponse, ex, nameof(LogOut));
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetUserDto>> DeleteUser(int Id)
        {
            var serviceResponse = new ServiceResponse<GetUserDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _User = _unitOfWork.GetRepository<User>();
                var result = await _User.GetByIdAsync(Id);
                if (result != null)
                {
                    if (result.IsDeleted)
                    {
                        serviceResponse.Data = _mapper.Map<GetUserDto>(result);
                        serviceResponse.Message = $"{GetUserRole()} has already been deleted.";
                    }
                    else
                    {
                        result.IsDeleted = true;
                        //result.ModifiedBy = GetUserId();
                        result.ModifiedBy = 1;
                        result.ModifiedAt = DateTime.Now;
                        await _User.UpdateAsync(result);
                        await _unitOfWork.SaveChangesAsync();
                        await _unitOfWork.CommitAsync();
                        serviceResponse.Data = _mapper.Map<GetUserDto>(result);
                        serviceResponse.Message = $"{GetUserRole()} deleted successfully.";
                    }
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"{GetUserRole()} not found.";
                }
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.GetType().ToString();
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(DeleteUser)}");
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetOtpDto>> ValidateEmail(AddOtpDto addOtpDto)
        {
            var serviceResponse = new ServiceResponse<GetOtpDto>();
            try
            {
                if (await EmailExists(addOtpDto.Email))
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Email already exist.";
                    return serviceResponse;
                }
                else
                {
                    var OTP = GenerateAndBuildOtp(addOtpDto);
                    // encrypt otp
                    string hash = "Salaf Technocrats";
                    string otpenc;
                    byte[] data = UTF8Encoding.UTF8.GetBytes(OTP.OTP);
                    using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
                    {
                        byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
                        using (TripleDESCryptoServiceProvider tripdes = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                        {
                            ICryptoTransform transform = tripdes.CreateEncryptor();
                            byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                            otpenc = Convert.ToBase64String(results, 0, results.Length);
                        }
                    }
                    var add = new Otp()
                    {
                        FirstName = OTP.FirstName,
                        LastName = OTP.LastName,
                        Email = OTP.Email,
                        MobileNumber = OTP.MobileNumber,
                        StartTime = OTP.StartTime,
                        ExpiryTime = OTP.ExpiryTime,
                        OTP = otpenc,
                    };

                    await _otpRepo.AddAsync(add);
                    await _unitOfWork.CommitAsync();
                    // send Otp
                    string webRootPath = _configuration["Web:Path"];
                    string folderName = string.Empty;
                    folderName = "EmailTemplates/SignUp.html";//CRC: move this path to config
                    string template = Path.Combine(webRootPath, folderName);
                    StreamReader str = new StreamReader(template);
                    string Body = str.ReadToEnd();
                    str.Close();
                    string VendorName = addOtpDto.FirstName + " " + addOtpDto.LastName;
                    Body = Body.Replace("[Vendor Name]", VendorName);
                    Body = Body.Replace("[OTP]", OTP.OTP);
                    var objGetEmailDto = new GetEmailDto
                    {
                        To = addOtpDto.Email,
                        Subject = "Otp for signup",
                        Body = Body
                    };
                    await _emailService.SendEmailAsync(objGetEmailDto);
                    serviceResponse.Success = true;
                    serviceResponse.Message = "Otp sent to given emailId.";
                    serviceResponse.Data = OTP;
                    return serviceResponse;
                }
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.GetType().ToString();
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(ValidateEmail)}");
            }
            return serviceResponse;
        }

        private static GetOtpDto GenerateAndBuildOtp(AddOtpDto addOtpDto)
        {
            string range = "0123456789";
            int len = range.Length;
            string otp = string.Empty;
            int otplen = 6;
            string code;
            int getIndex;
            for (int i = 0; i < otplen; i++)
            {
                do
                {
                    getIndex = new Random().Next(0, len);
                    code = range.ToCharArray()[getIndex].ToString();
                } while (otp.IndexOf(code) != -1);
                otp += code;
            }

            var Date1 = new DateTime();
            DateTime Date2 = Date1.AddMinutes(10);
            GetOtpDto add = new GetOtpDto()
            {
                FirstName = addOtpDto.FirstName,
                LastName = addOtpDto.LastName,
                Email = addOtpDto.Email,
                MobileNumber = addOtpDto.MobileNumber,
                StartTime = Date1.TimeOfDay,
                ExpiryTime = Date2.TimeOfDay,
                OTP = otp,
            };
            return add;
        }

        public async Task<bool> EmailExists(string email)
        {
            var _userRepo = _unitOfWork.GetRepository<User>();
            var data = await _userRepo.Search(
                x => x.Email.ToLower() == email.ToLower());
            bool Exist = data.Any();
            if (Exist)
            {
                return true;
            }
            return false;
        }

        public async Task<ServiceResponse<GetOtpDto>> ValidateEmailForPasswordRecovery(AddOtpDto addOtpDto)
        {
            var serviceResponse = new ServiceResponse<GetOtpDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                if (await EmailExists(addOtpDto.Email))
                {
                    var _User = _unitOfWork.GetRepository<User>();
                    var user = await _User.Search(x => x.Email.ToLower() == addOtpDto.Email.ToLower());
                    addOtpDto.FirstName = user.FirstOrDefault().FirstName;
                    addOtpDto.LastName = user.FirstOrDefault().LastName;
                    addOtpDto.MobileNumber = user.FirstOrDefault().MobileNumber;

                    var OTP = GenerateAndBuildOtp(addOtpDto);
                    // encrypt otp
                    string hash = "Ahmed Mart";
                    string otpenc;
                    byte[] data = UTF8Encoding.UTF8.GetBytes(OTP.OTP);
                    using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
                    {
                        byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
                        using (TripleDESCryptoServiceProvider tripdes = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                        {
                            ICryptoTransform transform = tripdes.CreateEncryptor();
                            byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                            otpenc = Convert.ToBase64String(results, 0, results.Length);
                        }
                    }
                    var add = new Otp()
                    {
                        FirstName = OTP.FirstName,
                        LastName = OTP.LastName,
                        Email = OTP.Email,
                        MobileNumber = OTP.MobileNumber,
                        StartTime = OTP.StartTime,
                        ExpiryTime = OTP.ExpiryTime,
                        OTP = otpenc,
                    };

                    await _otpRepo.AddAsync(add);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();
                    // send Otp
                    string webRootPath = _configuration["Web:Path"];
                    string folderName = string.Empty;
                    folderName = "EmailTemplates/PasswordRecoveryTemplate.html";//CRC: move this path to config
                    string template = Path.Combine(webRootPath, folderName);
                    StreamReader str = new StreamReader(template);
                    string Body = str.ReadToEnd();
                    str.Close();
                    string VendorName = addOtpDto.FirstName + " " + addOtpDto.LastName;
                    Body = Body.Replace("[Vendor Name]", VendorName);
                    Body = Body.Replace("[OTP]", OTP.OTP);
                    var objGetEmailDto = new GetEmailDto
                    {
                        To = addOtpDto.Email,
                        Subject = "Otp for password recovery",
                        Body = Body
                    };
                    await _emailService.SendEmailAsync(objGetEmailDto);
                    serviceResponse.Success = true;
                    serviceResponse.Message = "Otp sent to given emailId.";
                    serviceResponse.Data = OTP;
                    return serviceResponse;
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Email does not exist.";
                    return serviceResponse;
                }
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.GetType().ToString();
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(ValidateEmailForPasswordRecovery)}");
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetUserDto>> UpdatePassword(UpdateUserDto updateUserDto)
        {
            var serviceResponse = new ServiceResponse<GetUserDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _User = _unitOfWork.GetRepository<User>();
                var result = await _User.Search(x => x.Email == updateUserDto.Email);
                var user = result.FirstOrDefault();
                if (user != null)
                {
                    if (!string.IsNullOrEmpty(updateUserDto.Password))
                    {
                        PasswordHashSaltUtility(updateUserDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
                        user.PasswordHash = passwordHash;
                        user.PasswordSalt = passwordSalt;
                    }
                    await _User.UpdateAsync(user);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();
                    serviceResponse.Data = _mapper.Map<GetUserDto>(user);
                    serviceResponse.Message = $"{GetUserRole()} updated successfully.";
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"{GetUserRole()} not found.";
                }
            }
            catch (Exception ex)
            {
                await RollbackAndHandleException(serviceResponse, ex, nameof(LogOut));
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<bool>> LogOut()
        {
            var serviceResponse = new ServiceResponse<bool>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _UserTracker = _unitOfWork.GetRepository<UserTracker>();
                var result = await _UserTracker.GetByIdAsync(GetUserTrackerId());
                if (result != null)
                {
                    result.LogOut = DateTime.Now.TimeOfDay;
                    result.Status = false;
                    //result.ModifiedBy = GetUserId();
                    result.ModifiedBy = 1;
                    result.ModifiedAt = DateTime.Now;
                    await _UserTracker.UpdateAsync(result);
                    await _unitOfWork.SaveChangesAsync();
                    //other table insert Update
                    await _unitOfWork.CommitAsync();
                    serviceResponse.Message = $"User logged out successfully.";
                }
            }
            catch (Exception ex)
            {
                await RollbackAndHandleException(serviceResponse, ex, nameof(LogOut));
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<string>> LoginThroughMobile(GetLoginThroughMobileDto loginDto)
        {
            var serviceResponse = new ServiceResponse<string>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _userRepo = _unitOfWork.GetRepository<User>();
                var result = await _userRepo.GetFirstOrDefaultAsync(x => x.MobileNumber == loginDto.MobileNumber, x => x.Role);

                if (result != null)
                {
                    var addOtpDto = new AddOtpDto
                    {
                        FirstName = result.FirstName,
                        LastName = result.LastName,
                        Email = result.Email,
                        MobileNumber = result.MobileNumber
                    };

                    var otpResponse = await GenerateAndSendOtp(addOtpDto);

                    if (otpResponse.Success)
                    {
                        await _unitOfWork.CommitAsync(); 
                        serviceResponse.Data = "OTP sent to your mobile number.";
                        serviceResponse.Success = true;
                    }
                    else
                    {
                        await _unitOfWork.RollbackAsync();
                        serviceResponse.Success = false;
                        serviceResponse.Message = "Failed to send OTP.";
                    }
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Mobile number not found.";
                }
            }
            catch (Exception ex)
            {
                await RollbackAndHandleException(serviceResponse, ex, nameof(LoginThroughMobile));
            }
            return serviceResponse;
        }

        private async Task<ServiceResponse<GetOtpDto>> GenerateAndSendOtp(AddOtpDto addOtpDto)
        {
            var serviceResponse = new ServiceResponse<GetOtpDto>();
            try
            {
                var OTP = GenerateAndBuildOtps(addOtpDto);

                // Encrypt OTP
                string otpenc = EncryptOtp(OTP.OTP);
                var otpEntity = new Otp
                {
                    FirstName = OTP.FirstName,
                    LastName = OTP.LastName,
                    Email = OTP.Email,
                    MobileNumber = OTP.MobileNumber,
                    StartTime = OTP.StartTime,
                    ExpiryTime = OTP.ExpiryTime,
                    OTP = otpenc,
                };

                await _otpRepo.AddAsync(otpEntity);
                await _unitOfWork.SaveChangesAsync();

                // Create an instance of GetSmsDto with the recipient's phone number and the OTP message
                var smsDto = new GetSmsDto
                {
                    To = addOtpDto.MobileNumber,
                    Message = $" Your OTP For Ahmad Mart Account is {OTP.OTP}  " +
                    $"Do not share this OTP with anyone for security reasons. AHMAD ENTERPRISES "
                };

                // Call the SendSmsAsync method on your SMS service
                var smsResponse = await _smsService.SendSmsAsync(smsDto);
                if (smsResponse.Success)
                {
                    serviceResponse.Success = true;
                    serviceResponse.Message = "OTP sent successfully!";
                    serviceResponse.Data = OTP;
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Failed to send OTP via SMS: " + smsResponse.Message;
                }
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.GetType().ToString();
                _logger.LogError(ex, $"Something went wrong in {nameof(GenerateAndSendOtp)}");
            }
            return serviceResponse;
        }

        private string EncryptOtp(string otp)
        {
            string hash = "Ahmed Mart";
            byte[] data = UTF8Encoding.UTF8.GetBytes(otp);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
                using (TripleDESCryptoServiceProvider tripdes = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = tripdes.CreateEncryptor();
                    byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                    return Convert.ToBase64String(results);
                }
            }
        }

        private static GetOtpDto GenerateAndBuildOtps(AddOtpDto addOtpDto)
        {
            string range = "0123456789";
            int len = range.Length;
            string otp = string.Empty;
            int otplen = 6;
            string code;
            int getIndex;
            for (int i = 0; i < otplen; i++)
            {
                getIndex = new Random().Next(0, len);
                code = range.ToCharArray()[getIndex].ToString();
                otp += code;
            }

            DateTime startTime = DateTime.Now;
            DateTime expiryTime = startTime.AddMinutes(10);

            return new GetOtpDto
            {
                FirstName = addOtpDto.FirstName,
                LastName = addOtpDto.LastName,
                Email = addOtpDto.Email,
                MobileNumber = addOtpDto.MobileNumber,
                StartTime = startTime.TimeOfDay,
                ExpiryTime = expiryTime.TimeOfDay,
                OTP = otp,
            };
        }

        public async Task<ServiceResponse<GetOtpDto>> GetSingleOtp(int Id)
        {
            var serviceResponse = new ServiceResponse<GetOtpDto>();
            try
            {
                var repository = _unitOfWork.GetRepository<Otp>();
                var result = await repository.GetByIdAsync(Id);
                var data = _mapper.Map<GetOtpDto>(result);
                serviceResponse.Data = data;
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(GetSingleOtp));
            }
            return serviceResponse;
        }
        public async Task<ServiceResponse<GetUserDto>> VerifyOtpAndLogin(VerifyOtpDto verifyOtpDto)
        {
            var serviceResponse = new ServiceResponse<GetUserDto>();
            try
            {
                var currentDateTime = DateTime.Now;
                var currentDate = currentDateTime.Date;
                var currentTime = currentDateTime.TimeOfDay;

                var otpEntities = await _otpRepo.SearchAsync(x => x.MobileNumber == verifyOtpDto.MobileNumber  && x.CreatedAt.Date == currentDate);
                var otpEntity = otpEntities
                    .Where(x => x.ExpiryTime > currentTime)
                    .OrderByDescending(x => x.ExpiryTime)
                    .FirstOrDefault();

                if (otpEntity == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "The OTP provided is invalid or does not exist. Please request a new OTP and try again.";
                    return serviceResponse;
                }

                // Decrypt OTP
                string decryptedOtp = DecryptOtp(otpEntity.OTP);

                if (decryptedOtp != verifyOtpDto.OTP)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "The OTP provided is incorrect. Please try with correct OTP or request a new OTP.";
                    return serviceResponse;
                }

                    var _userRepo = _unitOfWork.GetRepository<User>();
                    var user = await _userRepo.GetFirstOrDefaultAsync(x => x.MobileNumber == verifyOtpDto.MobileNumber, x => x.Role);
                    var Userdata = _mapper.Map<GetUserDto>(user);

                    var _userTrackerRepo = _unitOfWork.GetRepository<UserTracker>();
                    var userTracker = await _userTrackerRepo.GetFirstOrDefaultAsync(x => x.UserID == user.ID && x.Status == true);
                    if (userTracker != null)
                    {
                        userTracker.Status = false;
                        userTracker.ModifiedBy = Userdata.ID;
                        userTracker.ModifiedAt = DateTime.Now;
                        await _userTrackerRepo.UpdateAsync(userTracker);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    var objUserTracker = new UserTracker()
                    {
                        UserID = Userdata.ID,
                        LogIn = DateTime.Now.TimeOfDay,
                        CreatedBy = user.ID,
                        CreatedAt = DateTime.Now
                    };
                    await _userTrackerRepo.AddAsync(objUserTracker);
                    await _unitOfWork.SaveChangesAsync();
                    user.Token = CreateToken(user, objUserTracker);
                //added
                     var refreshToken = GenerateRefreshToken();
                     SetRefreshToken(user, refreshToken);
                //stop
                await _userRepo.UpdateAsync(user);
                    await _unitOfWork.SaveChangesAsync();
                    //await _unitOfWork.CommitAsync();

                    Userdata.Token = user.Token;
                    serviceResponse.Data = Userdata;
                    serviceResponse.Message = $"User logged in successfully.";

            }
            catch (Exception ex)
            {
                await RollbackAndHandleException(serviceResponse, ex, $"Something Went Wrong in the {ex} {nameof(VerifyOtpAndLogin)}");

            }
            return serviceResponse;
        }


        private string DecryptOtp(string encryptedOtp)
        {
            string hash = "Ahmed Mart";
            byte[] data = Convert.FromBase64String(encryptedOtp);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
                using (TripleDESCryptoServiceProvider tripdes = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = tripdes.CreateDecryptor();
                    byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                    return UTF8Encoding.UTF8.GetString(results);
                }
            }
        }
        //Re Send otp
        public async Task <ServiceResponse<GetOtpDto>> ResendOtp(ResendOtpDto rsndOtp)
        {
            var serviceResponse = new ServiceResponse<GetOtpDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _otpRepo = _unitOfWork.GetRepository<Otp>();
                var currentDateTime = DateTime.Now;
                var currentDate = currentDateTime.Date;
                var currentTime = currentDateTime.TimeOfDay;
                var OtpEntities = await _otpRepo.SearchAsync(x=>x.MobileNumber == rsndOtp.MobileNumber && x.CreatedAt.Date== currentDate);
                var OtpEntity = OtpEntities.Where(x => x.ExpiryTime > currentTime)
                    .OrderByDescending(x => x.ExpiryTime).FirstOrDefault();
                if(OtpEntity != null)
                {
                    //sending Same Otp Wich is Not Expire
                    var objGetSmsDto = new GetSmsDto
                    {
                        To = rsndOtp.MobileNumber,
                        Message = $"Your OTP is {DecryptOtp(OtpEntity.OTP)}"
                    };
                    await _smsService.SendSmsAsync(objGetSmsDto);

                    serviceResponse.Success = true;
                    serviceResponse.Message = "OTP sent to your mobile number.";
                    serviceResponse.Data = new GetOtpDto
                    {
                        FirstName = OtpEntity.FirstName,
                        LastName = OtpEntity.LastName,
                        Email = OtpEntity.Email,
                        MobileNumber = OtpEntity.MobileNumber,
                        //OTP = DecryptOtp(OtpEntity.OTP),
                        StartTime = OtpEntity.StartTime,
                        ExpiryTime = OtpEntity.ExpiryTime
                    };
                }
                else
                {
                    var _userRepo = _unitOfWork.GetRepository<User>();
                    var result = await _userRepo.GetFirstOrDefaultAsync(x => x.MobileNumber == rsndOtp.MobileNumber);
                    var newOtpResponse = await GenerateAndSendOtp(new AddOtpDto { 
                        MobileNumber = rsndOtp.MobileNumber,
                        FirstName=result.FirstName ,
                        LastName=result.LastName ,
                        Email=result.Email 
                    });
                    if (newOtpResponse.Success)
                    {
                        serviceResponse.Success = true;
                        serviceResponse.Message = "A new OTP has been sent to your mobile number.";
                        serviceResponse.Data = newOtpResponse.Data;
                    }
                    else
                    {
                        serviceResponse.Success = false;
                        serviceResponse.Message = "Failed to generate and send a new OTP.";
                    }

                }

            }
            catch (Exception ex)
            {
                await RollbackAndHandleException(serviceResponse, ex, nameof(ResendOtp));
            }
            return serviceResponse;
        }

        //refresh token
        public async Task<ServiceResponse<bool>> RefreshTokenAsync()
        {
            var serviceResponse = new ServiceResponse<bool>();
            try
            {
                var httpRequest = _httpContextAccessor.HttpContext!.Request;
                var refreshToken = httpRequest.Cookies["refreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                {
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.Unauthorized;
                    serviceResponse.Message = $"Refresh token not found.";
                    return serviceResponse;
                }
                var _userRepo = _unitOfWork.GetRepository<User>();
                var resultUser = await _userRepo.GetFirstOrDefaultAsync(x => x.RefreshToken.Equals(refreshToken));
                if (resultUser == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.Unauthorized;
                    serviceResponse.Message = $"Invalid Refresh token.";
                    return serviceResponse;
                }
                else if (resultUser.RefreshTokenTokenExpiresAt < DateTime.UtcNow)
                {
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.Unauthorized;
                    serviceResponse.Message = $"Refresh token expired.";
                    return serviceResponse;
                }
                //using var transaction = _unitOfWork.BeginTransactionAsync();
                //var dataUserManagement = _mapper.Map<AddUserDto>(resultUser);
                //var userTrackerId = GetUserTrackerId();
                // dataUserManagement.UserTrackerID = userTrackerId;
                using var transaction =  _unitOfWork.BeginTransactionAsync();

                // Fetch or create UserTracker
                var _userTrackerRepo = _unitOfWork.GetRepository<UserTracker>();
                var userTracker = await _userTrackerRepo.GetFirstOrDefaultAsync(ut => ut.UserID == resultUser.ID && ut.LogOut == default);
                // If no active session is found, create a new UserTracker entry
                if (userTracker == null)
                {
                    userTracker = new UserTracker
                    {
                        UserID = resultUser.ID,
                        LogIn = DateTime.UtcNow.TimeOfDay,
                        CreatedBy = resultUser.ID,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _userTrackerRepo.AddAsync(userTracker);
                    await _unitOfWork.SaveChangesAsync();
                }
                string token = CreateToken(resultUser, userTracker);
                var newRefreshToken = GenerateRefreshToken();
                SetRefreshToken(resultUser, newRefreshToken);
                await _userRepo.UpdateAsync(resultUser);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                serviceResponse.Data = true;
            }
            catch (Exception ex)
            {
                await RollbackAndHandleException(serviceResponse, ex, nameof(RefreshTokenAsync));
            }
            return serviceResponse;
        }
    }
}
