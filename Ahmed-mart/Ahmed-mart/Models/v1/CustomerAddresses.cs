using System.ComponentModel.DataAnnotations;

namespace Ahmed_mart.Models.v1
{
    public class CustomerAddresses :IEntityBase
    {
        public int ID { get; set; }
        public int StoreID { get; set; }
        public int CustomersID { get; set; }
        public virtual Customers Customers { get; set; }
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
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
