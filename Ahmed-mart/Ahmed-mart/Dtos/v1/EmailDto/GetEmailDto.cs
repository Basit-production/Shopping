using Ahmed_mart.Dtos.v1.SmtpDetailsDto;

namespace Ahmed_mart.Dtos.v1.EmailDto
{
    public class GetEmailDto
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public IList<IFormFile>? Attachments { get; set; }
        public virtual GetSmtpDetailsDto SmtpDetails { get; set; }
    }
}
