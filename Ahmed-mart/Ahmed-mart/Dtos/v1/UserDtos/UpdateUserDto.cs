namespace Ahmed_mart.Dtos.v1.UserDtos
{
    public class UpdateUserDto
    {
        public int ID { get; set; }
        public int? ParentUserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        public bool Status { get; set; }
        public IFormFile File { get; set; }
    }
}
