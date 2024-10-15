namespace Ahmed_mart.Dtos.v1.CustomerUsersDto
{
    public class AddCustomerUsersDto
    {
        public int StoreID { get; set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int CustomersID { get; set; }
        public string OTP { get; set; }
        public bool Status { get; set; } = true;
    }
}
