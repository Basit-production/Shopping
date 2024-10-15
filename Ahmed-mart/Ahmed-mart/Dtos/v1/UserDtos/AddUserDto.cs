using Ahmed_mart.Dtos.v1.StoreDto;

namespace Ahmed_mart.Dtos.v1.UserDtos
{
    public class AddUserDto
    {
        public int? ParentUserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        public int RoleID { get; set; }
        public bool Status { get; set; } = true;
        //public virtual AddStoreDto Store { get; set; }//Need to DIcuss
        public IFormFile? File { get; set; }
        //Added
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime? RefreshTokenCreatedAt { get; set; }
        public DateTime? RefreshTokenTokenExpiresAt { get; set; }
        //public int UserTrackerID { get; set; }
    }
}
