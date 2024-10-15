namespace Ahmed_mart.Dtos.v1.AdminDtos
{
    public class UpdateAdminDto
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        public bool Status { get; set; }
        public int SignupID { get; set; }
        public int OrderID { get; set; }
        public int ReturnPolicyID { get; set; }
        public IFormFile? File { get; set; }
    }
}
