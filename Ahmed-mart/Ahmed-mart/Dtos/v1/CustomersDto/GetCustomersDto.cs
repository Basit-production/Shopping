using Ahmed_mart.Dtos.v1.CustomerAddressesDto;

namespace Ahmed_mart.Dtos.v1.CustomersDto
{
    public class GetCustomersDto
    {
        public int ID { get; set; }
        public int StoreID { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public int CustomerGroupID { get; set; }
        public string Organization { get; set; }
        public string GSTIN { get; set; }
        public decimal Credits { get; set; }
        public decimal Wallet { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public bool IsLocked { get; set; }
        public virtual ICollection<GetCustomerAddressesDto> CustomerAddresses { get; set; } = new List<GetCustomerAddressesDto>();
    }
}
