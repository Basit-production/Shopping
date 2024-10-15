using System.ComponentModel.DataAnnotations;
using System.Timers;

namespace Ahmed_mart.Models.v1
{
    public class Customers : IEntityBase
    {
        public int ID { get; set; }
        public int StoreID { get; set; }
        public virtual Store Store { get; set; }
        public string? Path { get; set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string GSTIN { get; set; }
        public int CustomerGroupID { get; set; }
        public int CustomerType { get; set; }
        public decimal Wallet { get; set; }
        public decimal Credits { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual ICollection<CustomerAddresses> CustomerAddresses { get; set; } = new List<CustomerAddresses>();
    }
}
