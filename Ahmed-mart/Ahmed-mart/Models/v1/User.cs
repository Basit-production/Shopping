using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ahmed_mart.Models.v1
{
    public class User: IEntityBase
    {
        public int ID { get; set; }
        [ForeignKey("ParentUser")]
        public int? ParentUserID { get; set; }
        public virtual User ParentUser { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string? Token { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public int RoleID { get; set; }
        public virtual Role Role { get; set; }
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
        //added
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime? RefreshTokenCreatedAt { get; set; }
        public DateTime? RefreshTokenTokenExpiresAt { get; set; }
    }
}
