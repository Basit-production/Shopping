using Ahmed_mart.Models.v1;

namespace Ahmed_mart.Dtos.v1.AdminDtos
{
    public class GetAdminDto
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string Token { get; set; }
        public int RoleID { get; set; }
        public virtual Role Role { get; set; }
        public int SignupID { get; set; }
        public int OrderID { get; set; }
        //public int ReturnPolicyId { get; set; }// need to discuss
        public bool IsLocked { get; set; }
        public string? Path { get; set; }
        public bool Status { get; set; } = true;
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
