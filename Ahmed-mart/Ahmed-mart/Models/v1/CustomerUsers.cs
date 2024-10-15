using System.ComponentModel.DataAnnotations;

namespace Ahmed_mart.Models.v1
{
    public class CustomerUsers : IEntityBase
    {
        public int ID { get; set; }
        public int StoreID { get; set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string? Token { get; set; }
        //added
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime? RefreshTokenCreatedAt { get; set; }
        public DateTime? RefreshTokenTokenExpiresAt { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public int CustomersID { get; set; }
        public virtual Customers Customers { get; set; }
        public bool IsLocked { get; set; }
        public string? Path { get; set; }
        public bool Status { get; set; } = true;
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
