using Ahmed_mart.Dtos.v1.SmsDtos;
using Ahmed_mart.Repository.v1;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace Ahmed_mart.Services.v1.SmsService
{
    public class SmsService : ISmsService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmsService> _logger;
        private readonly SmsApiClient _smsApiClient;

        public SmsService(IConfiguration configuration, ILogger<SmsService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            // Instantiate SmsApiClient with the base URL from configuration
            string baseUrl = _configuration["Sms:ProviderUrl"];
            _smsApiClient = new SmsApiClient(baseUrl);
        }

        public async Task<ServiceResponse<GetSmsDto>> SendSmsAsync(GetSmsDto getSmsDto)
        {
            var serviceResponse = new ServiceResponse<GetSmsDto>();
            try
            {
                string userId = _configuration["Sms:UserID"];
                string password = _configuration["Sms:Password"];
                string senderId = _configuration["Sms:SenderID"];
                string entityId = _configuration["Sms:EntityID"];
                string templateId = _configuration["Sms:TemplateID"];
               // string phoneNumber = _configuration["Sms:MobileNo"];

                //string formattedPhoneNumber = FormatPhoneNumber(getSmsDto.To);
                string phoneNumber = getSmsDto.To;

                _logger.LogInformation($"Sending SMS to {phoneNumber} with message: {getSmsDto.Message}");

                // Send SMS using the updated SmsApiClient
                string response = await _smsApiClient.SendSingleSmsAsync(userId, password, senderId, phoneNumber, getSmsDto.Message, entityId, templateId);
                _logger.LogInformation($"SMS API Response: {response}");

                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response);

                if (jsonResponse != null && jsonResponse.Status == "OK")
                {
                    serviceResponse.Data = getSmsDto;
                    serviceResponse.Message = "SMS sent successfully.";
                    serviceResponse.Success = true;
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Failed to send SMS.";
                    _logger.LogError($"Failed to send SMS. Response: {response}");
                }
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.GetType().ToString();
                _logger.LogError(ex, "Error sending SMS.");
            }

            return serviceResponse;
        }

        //private string FormatPhoneNumber(string phoneNumber)
        //{
        //    if (!phoneNumber.StartsWith("+91"))
        //    {
        //        phoneNumber = "+91" + phoneNumber;
        //    }
        //    return phoneNumber;
        //}
    }

}
