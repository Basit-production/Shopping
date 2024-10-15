namespace Ahmed_mart.Dtos.v1.CustomerAddressesDto
{
    public class AddCustomerAddressesDto
    {
        public int StoreID { get; set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string PINCode { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string LandMark { get; set; }
        public string City { get; set; }
        public int StateID { get; set; }
        public int AddressType { get; set; }
        public bool Status { get; set; }
        public bool IsDefault { get; set; }
        public bool IsDeleted { get; set; }
        public int CustomerID { get; set; }
        public int CreatedBy { get; set; }
    }
}
