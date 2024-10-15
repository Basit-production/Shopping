using Ahmed_mart.Dtos.v1.CustomersDto;

namespace Ahmed_mart.Dtos.v1.CustomerUsersDto
{
    public class GetCustomerUsersDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public int CustomersId { get; set; }
        public virtual GetCustomersDto Customers { get; set; }
        public bool IsLocked { get; set; }
        public string Path { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
