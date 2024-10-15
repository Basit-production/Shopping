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
using Amazon.Runtime.Internal;
using AutoMapper;
using Azure.Core;
using k8s.KubeConfigModels;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Ahmed_mart.Services.v1.AdminAuthService
{
    public class AdminAuthService : BaseService, IAdminAuthService
    {
        protected override string CacheKey => "adminauthCacheKey";
        private readonly ISmsService _smsService;
        private readonly IFileService _fileService;
        private readonly IEmailService _emailService;
        private readonly IGenericRepository<Otp> _otpRepo;

        public AdminAuthService(
            IFileService fileService,
            ISmsService smsService,
            IEmailService emailService,
            IGenericRepository<Otp> otpRepo,
            IUnitOfWork unitOfWork,
            ILogger<BaseService> logger,
            IMapper mapper,
            IMemoryCache memoryCache,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration) :
            base(unitOfWork, logger, mapper, memoryCache, httpContextAccessor, configuration)
        {
            _smsService = smsService;
            _fileService = fileService;
            _emailService = emailService;
            _otpRepo = otpRepo;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User
            .FindFirstValue(ClaimTypes.NameIdentifier)!);

        private int GetUserTrackerId() => int.Parse(_httpContextAccessor.HttpContext!.User
            .FindFirstValue("UserTrackerID")!);
         private string GetUserRole() => _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);

        public async Task<ServiceResponse<GetAdminDto>> Login(GetLoginDto loginDto)
        {
            var serviceResponse = new ServiceResponse<GetAdminDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _adminRepo = _unitOfWork.GetRepository<Admin>();
                var result = await _adminRepo.GetFirstOrDefaultAsync(
                    x => x.Email.ToLower() == loginDto.Email.ToLower() &&
                    x.Status == true,
                    x => x.Role);
                var data = _mapper.Map<GetAdminDto>(result);
                if (data == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    serviceResponse.Message = $"Admin not found.";
                    return serviceResponse;
                }
                else if (!VerifyPasswordHashSalt(loginDto.Password, result.PasswordHash, result.PasswordSalt))
                // else if (!VerifyPasswordHashSalt(loginDto.Password, result))
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"Wrong password.";
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return serviceResponse;
                }
                else if (data.IsLocked)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"Admin is locked.";
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return serviceResponse;
                }
                else if (data.IsDeleted)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"Admin is deleted.";
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return serviceResponse;
                }
                else
                {
                    var _userTrackerRepo = _unitOfWork.GetRepository<UserTracker>();//added
                    var userTracker = await _userTrackerRepo.GetFirstOrDefaultAsync(x => x.AdminID == result.ID && x.Status == true);
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
                        AdminID = result.ID,
                        LogIn = DateTime.Now.TimeOfDay,
                        CreatedBy = result.ID,
                        CreatedAt = DateTime.Now
                    };
                    await _userTrackerRepo.AddAsync(objUserTracker);
                    await _unitOfWork.SaveChangesAsync();
                    //await _unitOfWork.CommitAsync();
                     result.Token = CreateToken(result, objUserTracker);
                     await _adminRepo.UpdateAsync(result);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();

                    data.Token = result.Token;
                    serviceResponse.Data = data;
                    serviceResponse.Message = $"Admin logged in successfully.";
                }
            }
            catch (Exception ex)
            {
                await RollbackAndHandleException(serviceResponse, ex, nameof(Login));
            }
            return serviceResponse;
        }
        private bool VerifyPasswordHashSalt(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(Admin admin, UserTracker objUserTracker)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, admin.ID.ToString()),
                new Claim("UserTrackerId", objUserTracker.ID.ToString()),
                new Claim(ClaimTypes.Name, admin.Email),
                new Claim(ClaimTypes.Email, admin.Email),
                new Claim(ClaimTypes.Role, admin.Role.Name)
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
        public async Task<ServiceResponse<GetAdminDto>> AddAdmin(AddAdminDto addAdminDto)
        {
            var serviceResponse = new ServiceResponse<GetAdminDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _AdminRepo = _unitOfWork.GetRepository<Admin>();
                var Admindata = _mapper.Map<Admin>(addAdminDto);
                if (await UserExists(Admindata.ID, Admindata.Email))
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"Admin already exist.";
                    return serviceResponse;
                }
               
                PasswordHashSaltUtility(addAdminDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
                Admindata.PasswordHash = passwordHash;
                Admindata.PasswordSalt = passwordSalt;
                Admindata.RoleID = 1;//Admin
                Admindata.CreatedAt = DateTime.Now;
                Admindata.Token = "SampleToken";//should be Exclude Because This Should be Null in Admin Entity
                var data = await _AdminRepo.AddAsync(Admindata);//Added
                await _unitOfWork.SaveChangesAsync();//Added
                Admindata.CreatedBy = Admindata.ID;
                await _AdminRepo.UpdateAsync(Admindata);
                await _unitOfWork.SaveChangesAsync();
                if (addAdminDto.File != null && addAdminDto.File.Length > 0)
                {
                    var objUploadFileDto = new UploadFileDto()
                    {
                        Directory = $"Admin/AdminImage/{Admindata.ID}",
                        File = addAdminDto.File
                    };
                    var dataFileService = await _fileService.UploadFileAsync(objUploadFileDto);
                    if (dataFileService.Success)
                    {
                        Admindata.Path = dataFileService.Data.Path;
                        await _AdminRepo.UpdateAsync(Admindata);
                        await _unitOfWork.SaveChangesAsync();
                    }
                }
                await _unitOfWork.CommitAsync();
                serviceResponse.Data = _mapper.Map<GetAdminDto>(Admindata);
                serviceResponse.Message = $"Admin added successfully.";
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.GetType().ToString();
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(AddAdmin)}");
            }
            return serviceResponse;
        }
        //Update
        public async Task<ServiceResponse<GetAdminDto>> UpdateAdmin(UpdateAdminDto updateAdminDto)
        {
            var serviceResponse = new ServiceResponse<GetAdminDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _Admin = _unitOfWork.GetRepository<Admin>();
                var result = await _Admin.GetByIdAsync(updateAdminDto.ID);
                if (result != null)
                {
                    result.FirstName = updateAdminDto.FirstName;
                    result.LastName = updateAdminDto.LastName;
                    if (!string.IsNullOrEmpty(updateAdminDto.Email))
                    {
                        if (await UserExists(result.ID, updateAdminDto.Email))
                        {
                            serviceResponse.Success = false;
                            serviceResponse.Message = $"{GetUserRole()} already exist.";
                            return serviceResponse;
                        }
                        else
                        {
                            result.Email = updateAdminDto.Email;
                        }
                    }
                    result.MobileNumber = updateAdminDto.MobileNumber;
                    if (!string.IsNullOrEmpty(updateAdminDto.Password))
                    {
                        PasswordHashSaltUtility(updateAdminDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
                        result.PasswordHash = passwordHash;
                        result.PasswordSalt = passwordSalt;
                    }
                    result.Status = updateAdminDto.Status;
                    if (updateAdminDto.File != null && updateAdminDto.File.Length > 0)
                    {
                        var objUploadFileDto = new UploadFileDto()
                        {
                            Directory = $"Admin/AdminImage/{result.ID}",
                            File = updateAdminDto.File
                        };
                        var dataFileService = await _fileService.UploadFileAsync(objUploadFileDto);
                        if (dataFileService.Success)
                        {
                            result.Path = dataFileService.Data.Path;
                        }
                    }
                    //result.ModifiedBy = GetUserId();
                    result.ModifiedBy = 1;//need to remove after connect with UI
                    result.ModifiedAt = DateTime.Now;

                    var data = await _Admin.UpdateAsync(result);
                    await _unitOfWork.SaveChangesAsync();
                    // Other tbls inserts or updates
                    await _unitOfWork.CommitAsync();
                    serviceResponse.Data = _mapper.Map<GetAdminDto>(result);
                    serviceResponse.Message = $"{GetUserRole()} updated successfully.";
                    // Clear cache after success
                    _memoryCache.Remove(CacheKey);
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
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(UpdateAdmin)}");
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

        private async Task<bool> UserExists(int id, string email)
        {
            var _adminRepo = _unitOfWork.GetRepository<Admin>();
            var data = await _adminRepo.Search(
                x => x.ID != id &&
                x.Email.ToLower() == email.ToLower());
            bool Exist = data.Any();
            if (Exist)
            {
                return true;
            }
            return false;
        }

        public async Task<ServiceResponse<GetAdminDto>> LockUnlockAdmin(int Id)
        {
            var serviceResponse = new ServiceResponse<GetAdminDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _Admin = _unitOfWork.GetRepository<Admin>();
                var result= await _Admin.GetByIdAsync(Id);
                if (result != null)
                {
                    result.IsLocked = !result.IsLocked;
                   // result.ModifiedBy = GetUserId();
                    result.ModifiedBy = 1;
                    result.ModifiedAt = DateTime.Now;
                    var Admindata = await _Admin.UpdateAsync(result);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();
                    var data = _mapper.Map<GetAdminDto>(Admindata);
                    serviceResponse.Data = data;
                    serviceResponse.Message = $"{GetUserRole()} {(result.IsLocked ? "locked" : "unlocked")} successfully.";
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"{GetUserRole()} not found.";
                }
            }
            catch (Exception ex)
            {
                await RollbackAndHandleException(serviceResponse, ex, nameof(LockUnlockAdmin));
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetAdminDto>> UpdateAdminStatus(int Id)
        {
            var serviceResponse = new ServiceResponse<GetAdminDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _Admin = _unitOfWork.GetRepository<Admin>();
                var result = await _Admin.GetByIdAsync(Id);
                if (result != null)
                {
                    result.Status = !result.Status;
                   // result.ModifiedBy = GetUserId();
                    result.ModifiedBy = 1;
                    result.ModifiedAt = DateTime.Now;
                    var Admindata = await _Admin.UpdateAsync(result);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();
                    var data = _mapper.Map<GetAdminDto>(Admindata);
                    serviceResponse.Message = $"{GetUserRole()} {(data.Status ? "Activated" : "De-Activated")} successfully.";
                    serviceResponse.Data = data;
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"{GetUserRole()} not found.";
                }
            }
            catch (Exception ex)
            {
                await RollbackAndHandleException(serviceResponse, ex, nameof(LockUnlockAdmin));
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetAdminDto>> DeleteAdmin(int Id)
        {
            var serviceResponse = new ServiceResponse<GetAdminDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _Admin = _unitOfWork.GetRepository<Admin>();
                var result = await _Admin.GetByIdAsync(Id);
                if (result != null)
                {
                    if (result.IsDeleted)
                    {
                        var data = _mapper.Map<GetAdminDto>(result);
                        serviceResponse.Data = data;
                        serviceResponse.Message = $"{GetUserRole()} has already been deleted.";
                    }
                    else
                    {
                        result.IsDeleted = true;
                        //result.ModifiedBy = GetUserId();
                        result.ModifiedBy = 1;
                        result.ModifiedAt = DateTime.Now;
                        var Admindata = await _Admin.UpdateAsync(result);
                        await _unitOfWork.SaveChangesAsync();
                        await _unitOfWork.CommitAsync();
                        var data = _mapper.Map<GetAdminDto>(result);
                        serviceResponse.Data = data;
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
                await RollbackAndHandleException(serviceResponse, ex, nameof(LockUnlockAdmin));
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
            //using var transaction = _unitOfWork.BeginTransactionAsync();
            var _adminRepo= _unitOfWork.GetRepository<Admin>();
            var data = await _adminRepo.SearchAsync(
                 x => x.Email.ToLower() == email.ToLower()
                );
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
                    var _Admin = _unitOfWork.GetRepository<Admin>();
                    var user = await _Admin.Search(x => x.Email.ToLower() == addOtpDto.Email.ToLower());
                    addOtpDto.FirstName = user.FirstOrDefault().FirstName;
                    addOtpDto.LastName = user.FirstOrDefault().LastName;
                    addOtpDto.MobileNumber = user.FirstOrDefault().MobileNumber;

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
        public async Task<ServiceResponse<GetAdminDto>> UpdatePassword(UpdateAdminDto updateAdminDto)
        {
            var serviceResponse = new ServiceResponse<GetAdminDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _admin = _unitOfWork.GetRepository<Admin>();
                var result = await _admin.Search(x => x.Email == updateAdminDto.Email);
                var admin = result.FirstOrDefault();
                if (admin != null)
                {
                    if (!string.IsNullOrEmpty(updateAdminDto.Password))
                    {
                        PasswordHashSaltUtility(updateAdminDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
                        admin.PasswordHash = passwordHash;
                        admin.PasswordSalt = passwordSalt;
                    }
                    await _admin.UpdateAsync(admin);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();
                    serviceResponse.Data = _mapper.Map<GetAdminDto>(admin);
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
                await RollbackAndHandleException(serviceResponse, ex, nameof(UpdatePassword));
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
        //***********************
        public async Task<ServiceResponse<string>> LoginThroughMobile(GetLoginThroughMobileDto loginDto)
        {
            var serviceResponse = new ServiceResponse<string>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _adminRepo = _unitOfWork.GetRepository<Admin>();
                var result = await _adminRepo.GetFirstOrDefaultAsync(x => x.MobileNumber == loginDto.MobileNumber, x => x.Role);

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

        public async Task<ServiceResponse<GetAdminDto>> VerifyOtpAndLogin(VerifyOtpDto verifyOtpDto)
        {
            var serviceResponse = new ServiceResponse<GetAdminDto>();
            try
            {
                var currentDateTime = DateTime.Now;
                var currentDate = currentDateTime.Date;
                var currentTime = currentDateTime.TimeOfDay;

                var otpEntities = await _otpRepo.SearchAsync(x => x.MobileNumber == verifyOtpDto.MobileNumber && x.CreatedAt.Date == currentDate);
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

                var _adminRepo = _unitOfWork.GetRepository<Admin>();
                var admin = await _adminRepo.GetFirstOrDefaultAsync(x => x.MobileNumber == verifyOtpDto.MobileNumber, x => x.Role);
                var Userdata = _mapper.Map<GetAdminDto>(admin);

                var _userTrackerRepo = _unitOfWork.GetRepository<UserTracker>();
                var userTracker = await _userTrackerRepo.GetFirstOrDefaultAsync(x => x.AdminID == admin.ID && x.Status == true);
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
                    AdminID = admin.ID,
                    LogIn = DateTime.Now.TimeOfDay,
                    CreatedBy = admin.ID,
                    CreatedAt = DateTime.Now
                };
                await _userTrackerRepo.AddAsync(objUserTracker);
                await _unitOfWork.SaveChangesAsync();
                admin.Token = CreateToken(admin, objUserTracker);
                //added
                var refreshToken = GenerateRefreshToken();
                SetRefreshToken(admin, refreshToken);
                //stop
                await _adminRepo.UpdateAsync(admin);
                await _unitOfWork.SaveChangesAsync();
                //await _unitOfWork.CommitAsync();

                Userdata.Token = admin.Token;
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

        public async Task<ServiceResponse<GetOtpDto>> ResendOtp(ResendOtpDto rsndOtp)
        {
            var serviceResponse = new ServiceResponse<GetOtpDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _otpRepo = _unitOfWork.GetRepository<Otp>();
                var currentDateTime = DateTime.Now;
                var currentDate = currentDateTime.Date;
                var currentTime = currentDateTime.TimeOfDay;
                var OtpEntities = await _otpRepo.SearchAsync(x => x.MobileNumber == rsndOtp.MobileNumber && x.CreatedAt.Date == currentDate);
                var OtpEntity = OtpEntities.Where(x => x.ExpiryTime > currentTime)
                    .OrderByDescending(x => x.ExpiryTime).FirstOrDefault();
                if (OtpEntity != null)
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
                    var _adminRepo = _unitOfWork.GetRepository<Admin>();
                    var result = await _adminRepo.GetFirstOrDefaultAsync(x => x.MobileNumber == rsndOtp.MobileNumber);
                    var newOtpResponse = await GenerateAndSendOtp(new AddOtpDto
                    {
                        MobileNumber = rsndOtp.MobileNumber,
                        FirstName = result.FirstName,
                        LastName = result.LastName,
                        Email = result.Email
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
                var _adminRepo = _unitOfWork.GetRepository<Admin>();
                var resultUser = await _adminRepo.GetFirstOrDefaultAsync(x => x.RefreshToken.Equals(refreshToken));
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
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _userTrackerRepo = _unitOfWork.GetRepository<UserTracker>();
                var userTracker = await _userTrackerRepo.GetFirstOrDefaultAsync(ut => ut.AdminID == resultUser.ID && ut.LogOut == default);
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
                await _adminRepo.UpdateAsync(resultUser);
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
        private void SetRefreshToken(Admin admin, RefreshToken refreshToken)
        {
            var httpResponse = _httpContextAccessor.HttpContext!.Response;
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.Expires
            };
            httpResponse.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
            admin.RefreshToken = refreshToken.Token;
            admin.RefreshTokenCreatedAt = refreshToken.Created;
            admin.RefreshTokenTokenExpiresAt = refreshToken.Expires;
        }
    }
}
