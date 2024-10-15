namespace Ahmed_mart.Dtos.v1.OtpDto
{
    public class AddOtpDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? MobileNumber { get; set; }
        public string? OTP { get; set; }
        public int? StoreID { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan ExpiryTime { get; set; }
    }
}
