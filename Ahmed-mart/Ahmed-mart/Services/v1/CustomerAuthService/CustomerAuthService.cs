using Ahmed_mart.Dtos.v1;
using Ahmed_mart.Dtos.v1.CustomersDto;
using Ahmed_mart.Dtos.v1.CustomerUsersDto;
using Ahmed_mart.Dtos.v1.EmailDto;
using Ahmed_mart.Dtos.v1.OtpDto;
using Ahmed_mart.Models.v1;
using Ahmed_mart.Repository.v1;
using Ahmed_mart.Repository.v1.UnitOfWork;
using Ahmed_mart.Services.v1.EmailService;
using Ahmed_mart.Services.v1.FileService;
using Ahmed_mart.Services.v1.SmsService;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
namespace Ahmed_mart.Services.v1.CustomerAuthService
{
    public class CustomerAuthService:BaseService , ICustomerAuthService
    {
        protected override string CacheKey => "customerauthCacheKey";
        private readonly IFileService _fileService;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;

        public CustomerAuthService(IFileService fileService,
            IEmailService emailService,
            ISmsService smsService ,
            IUnitOfWork unitOfWork,
            ILogger<BaseService> logger,
            IMapper mapper,
            IMemoryCache memoryCache,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration
            ): base(unitOfWork, logger, mapper, memoryCache, httpContextAccessor, configuration)
        {
            _fileService = fileService;
            _emailService = emailService;
            _smsService = smsService;
        }
        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User
           .FindFirstValue(ClaimTypes.NameIdentifier)!);

        private int GetUserTrackerId() => int.Parse(_httpContextAccessor.HttpContext!.User
            .FindFirstValue("UserTrackerID")!);

        public async Task<ServiceResponse<GetCustomerUsersDto>> CustomerLogin(CustomerLoginDto customerLoginDto)
        {
            var serviceResponse = new ServiceResponse<GetCustomerUsersDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _customerRepo = _unitOfWork.GetRepository<Customers>();
                var result = await _customerRepo.GetFirstOrDefaultAsync(
                    x=>x.MobileNumber== customerLoginDto.MobileNumber ||
                    x.Email.ToLower().Equals(customerLoginDto.Email.ToLower()),
                    x => x.Store.User.Role);
                var data = _mapper.Map<GetCustomersDto>(result);
                //
                var _customerUsersRepo = _unitOfWork.GetRepository<CustomerUsers>();
                var customerLoging = await _customerUsersRepo.SearchAsync(x=>x.CustomersID==result.ID);
                if (customerLoging.Any())
                {
                    var login = customerLoging.FirstOrDefault();
                    if (!VerifyPasswordHashSalt(customerLoginDto.Password, login))
                    {
                        serviceResponse.Success = false;
                        serviceResponse.Message = $"Wrong password.";
                    }
                    else if (data.IsLocked)
                    {
                        serviceResponse.Success = false;
                        serviceResponse.Message = $"Customer is locked.";
                    }
                    else if (data.IsDeleted)
                    {
                        serviceResponse.Success = false;
                        serviceResponse.Message = $"Customer is deleted.";
                    }
                    else if(data.Status == false)
                    {
                        serviceResponse.Success = false;
                        serviceResponse.Message = $"Customer is inactive.";
                    }
                    else
                    {
                        var loggedUser = _mapper.Map<GetCustomerUsersDto>(login);
                        login.Token = CreateToken(login);
                        var refreshToken = GenerateRefreshToken();
                        SetRefreshToken(login, refreshToken);
                        await _customerUsersRepo.UpdateAsync(login);
                        await _unitOfWork.SaveChangesAsync();
                        await _unitOfWork.CommitAsync();
                        loggedUser.Token = login.Token;
                        serviceResponse.Data = loggedUser;
                        serviceResponse.Message = $"Customer logged in successfully.";
                    }
                } 

            }
            catch (Exception ex)
            {
                await RollbackAndHandleException(serviceResponse, ex, nameof(CustomerLogin));

            }
            return serviceResponse;
        }
         private static bool VerifyPasswordHashSalt(string password, CustomerUsers user)
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

        private void PasswordHashSaltUtility(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
        private string CreateToken(CustomerUsers user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Email, user.Email)
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
        private void SetRefreshToken(CustomerUsers customeruser, RefreshToken refreshToken)
        {
            var httpResponse = _httpContextAccessor.HttpContext!.Response;
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.Expires
            };
            httpResponse.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
            customeruser.RefreshToken = refreshToken.Token;
            customeruser.RefreshTokenCreatedAt = refreshToken.Created;
            customeruser.RefreshTokenTokenExpiresAt = refreshToken.Expires;
        }

        public async Task<ServiceResponse<GetOtpDto>> ValidateCustomerEmail(AddOtpDto addOtpDto)
        {
            var serviceResponse = new ServiceResponse<GetOtpDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _customerUsersRepo = _unitOfWork.GetRepository<CustomerUsers>();
                var result = await _customerUsersRepo.Search(x => (x.Email.ToLower() == addOtpDto.Email.ToLower() ||
                x.MobileNumber == addOtpDto.MobileNumber) && x.StoreID == addOtpDto.StoreID);
                bool Exist = result.Any();
                if (Exist)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Email or Phone number already exist.";
                    return serviceResponse;
                }
                else
                {
                    var OTP = GenerateAndBuildOtp(addOtpDto);
                    // encrypt otp
                    string hash = "Amamed Mart";
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
                    var _otpRepo = _unitOfWork.GetRepository<Otp>();
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
                    folderName = "EmailTemplates/SignUp.html";//CRC: move this path to config
                    string template = Path.Combine(webRootPath, folderName);
                    StreamReader str = new StreamReader(template);
                    string Body = str.ReadToEnd();
                    str.Close();
                    string VendorName = addOtpDto.FirstName;
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
                    //serviceResponse.Data = OTP;
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
        public async Task<ServiceResponse<GetOtpDto>> ValidateEmailForPasswordRecovery(AddOtpDto addOtpDto)
        {
            var serviceResponse = new ServiceResponse<GetOtpDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                if (await EmailExists(addOtpDto.Email))
                {
                    var _customerUsersRepo = _unitOfWork.GetRepository<CustomerUsers>();
                    var user = await _customerUsersRepo.Search(x => x.Email.ToLower() == addOtpDto.Email.ToLower());
                    addOtpDto.FirstName = user.FirstOrDefault().Name;
                    addOtpDto.LastName = user.FirstOrDefault().Name;
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
                    var _otpRepo = _unitOfWork.GetRepository<Otp>();
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
                    string VendorName = addOtpDto.FirstName;
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
        private async Task<bool> EmailExists(string email)
        {
            var _customerUsersRepo = _unitOfWork.GetRepository<CustomerUsers>();
            var data = await _customerUsersRepo.Search(
                x => x.Email.ToLower() == email.ToLower());
            bool Exist = data.Any();
            if (Exist)
            {
                return true;
            }
            return false;
        }
        public async Task<ServiceResponse<GetCustomerUsersDto>> UpdatePassword(UpdateCustomerUsersDto updateCustomerUsersDto)
        {
            var serviceResponse = new ServiceResponse<GetCustomerUsersDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _customerUsersRepo = _unitOfWork.GetRepository<CustomerUsers>();
                var objData = await _customerUsersRepo.Search(x => x.Email == updateCustomerUsersDto.Email);
                var data = objData.FirstOrDefault();
                if (data != null)
                {
                    if (!string.IsNullOrEmpty(updateCustomerUsersDto.Password))
                    {
                        PasswordHashSaltUtility(updateCustomerUsersDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
                        data.PasswordHash = passwordHash;
                        data.PasswordSalt = passwordSalt;
                    }
                    _customerUsersRepo.Update(data);
                    await _unitOfWork.CommitAsync();
                    serviceResponse.Data = _mapper.Map<GetCustomerUsersDto>(data);
                    serviceResponse.Message = $"Password updated successfully.";
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"Customer not found.";
                }
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.GetType().ToString();
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(UpdatePassword)}");
            }
            return serviceResponse;
        }
    }
}
