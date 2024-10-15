using Ahmed_mart.Dtos.v1.RoleDtos;

namespace Ahmed_mart.Dtos.v1.UserDtos
{
    public class GetUserDto
    {
        public int ID { get; set; }
        public int? ParentUserID { get; set; }
        public virtual GetUserDto ParentUser { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string? Token { get; set; }
        public int RoleId { get; set; }
        public virtual GetRoleDto Role { get; set; }
        public bool IsLocked { get; set; }
        public string? Path { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? RefreshToken { get; set; }
    }
}
