

using Ahmed_mart.Dtos.v1.EmailDto;
using Ahmed_mart.Repository.v1;
using Ahmed_mart.Repository.v1.UnitOfWork;
using Ahmed_mart.Services.v1.FileService;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Mail;
using System.Net;

namespace Ahmed_mart.Services.v1.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;

        public EmailService(
            ILogger<EmailService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<GetEmailDto>> SendEmailAsync(GetEmailDto getEmailDto)
        {
            var serviceResponse = new ServiceResponse<GetEmailDto>();
            try
            {
                using (var msg = new MailMessage())
                {
                    msg.From = new MailAddress(_configuration.GetSection("Email:From").Value, _configuration.GetSection("Email:DisplayName").Value);
                    msg.To.Add(getEmailDto.To);
                    msg.Subject = getEmailDto.Subject;
                    msg.Body = getEmailDto.Body;
                    msg.IsBodyHtml = true;
                    using (var smtp = new SmtpClient())
                    {
                        bool isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
                        if (isDevelopment)
                        {
                            smtp.Host = "localhost";
                            smtp.Port = 25;
                            smtp.EnableSsl = false;
                        }
                        else
                        {
                            smtp.Host = _configuration.GetSection("Email:Smtp:Host").Value;
                            smtp.Port = int.Parse(_configuration.GetSection("Email:Smtp:Port").Value);
                            smtp.EnableSsl = bool.Parse(_configuration.GetSection("Email:Smtp:EnableSsl").Value);
                        }
                        smtp.Credentials = new NetworkCredential(_configuration.GetSection("Email:Smtp:Username").Value, _configuration.GetSection("Email:Smtp:Password").Value);
                        await smtp.SendMailAsync(msg);
                    }
                }
                serviceResponse.Data = getEmailDto;
                serviceResponse.Message = $"Email sent successfully.";
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.GetType().ToString();
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(SendEmailAsync)}");
            }
            return serviceResponse;
        }
    }
}
