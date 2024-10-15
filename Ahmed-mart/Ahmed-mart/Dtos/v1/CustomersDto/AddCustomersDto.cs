using Ahmed_mart.Dtos.v1.CustomerAddressesDto;

namespace Ahmed_mart.Dtos.v1.CustomersDto
{
    public class AddCustomersDto
    {
        public int StoreID { get; set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        //public string Organization { get; set; }
        public string GSTIN { get; set; }
        public int CustomerGroupID { get; set; }
        public bool Status { get; set; }
        public IFormFile File { get; set; }
        public virtual ICollection<AddCustomerAddressesDto> CustomerAddresses { get; set; } = new List<AddCustomerAddressesDto>();
    }
}
