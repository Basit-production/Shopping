using Ahmed_mart.Dtos.v1.CustomersDto;

namespace Ahmed_mart.Dtos.v1.CustomerAddressesDto
{
    public class GetCustomerAddressesDto
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public int CustomersId { get; set; }
        public virtual GetCustomersDto Customers { get; set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string PINCode { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string LandMark { get; set; }
        public string City { get; set; }
        public int StateId { get; set; }
        public int AddressType { get; set; }
        public bool Status { get; set; }
        public bool IsDefault { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
