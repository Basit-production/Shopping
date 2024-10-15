namespace Ahmed_mart.Dtos.v1.AdminDtos
{
    public class AddAdminDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        public int SignupID { get; set; }
        public int OrderID { get; set; }
        public int ReturnPolicyID { get; set; }//Need to Discuss
        public IFormFile File { get; set; }//Need to Discuss
    }
}
