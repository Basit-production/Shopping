namespace Ahmed_mart.Dtos.v1.SmtpDetailsDto
{
    public class AddSmtpDetailsDto
    {
        public int? StoreID { get; set; }
        public string From { get; set; }
        public string DisplayName { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Status { get; set; } = true;
    }
}
