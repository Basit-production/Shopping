using System.Net;
using System.Web;

namespace Ahmed_mart.Services
{
    //public class SmsApiClient
    //{
    //    private readonly string _baseUrl;

    //    public SmsApiClient(string baseUrl)
    //    {
    //        _baseUrl = baseUrl;
    //    }

    //    public async Task<string> SendSingleSmsAsync(string userId, string password, string senderId, string phoneNumber, string message, string entityId, string templateId)
    //    {
    //        string encodedMessage = HttpUtility.UrlEncode(message);
    //        string encodedPassword = HttpUtility.UrlEncode(password);

    //        string url = $"{_baseUrl}?UserID={userId}&Password={encodedPassword}&SenderID={senderId}&Phno={phoneNumber}&Msg={encodedMessage}&EntityID={entityId}&TemplateID={templateId}";

    //        using (WebClient client = new WebClient())
    //        {

    //            string response = await client.DownloadStringTaskAsync(url);
    //            return response;
    //        }
    //    }
    //}
    public class SmsApiClient
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;

        public SmsApiClient(string baseUrl)
        {
            _baseUrl = baseUrl;
            _httpClient = new HttpClient(); // Ideally, inject HttpClient via DI for better control
        }

        public async Task<string> SendSingleSmsAsync(string userId, string password, string senderId, string phoneNumber, string message, string entityId, string templateId)
        {
            string encodedMessage = HttpUtility.UrlEncode(message);
            string encodedPassword = HttpUtility.UrlEncode(password);

            string url = $"{_baseUrl}?UserID={userId}&Password={encodedPassword}&SenderID={senderId}&Phno={phoneNumber}&Msg={encodedMessage}&EntityID={entityId}&TemplateID={templateId}";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                else
                {
                   
                    return $"Error: {response.StatusCode}, {response.ReasonPhrase}";
                }
            }
            catch (HttpRequestException ex)
            {
                
                return $"Exception occurred: {ex.Message}";
            }
        }
    }
}
