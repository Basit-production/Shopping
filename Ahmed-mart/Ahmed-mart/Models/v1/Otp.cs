using System.ComponentModel.DataAnnotations;

namespace Ahmed_mart.Models.v1
{
    public class Otp : IEntityBase
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public string MobileNumber { get; set; }
        public string OTP { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan ExpiryTime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }=DateTime.Now;
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
