using System.ComponentModel.DataAnnotations;

namespace Ahmed_mart.Models.v1
{
    public class CustomerCoupons :IEntityBase
    {
        public int ID { get; set; }
        public int CouponsID { get; set; }
        public virtual Coupons Coupons { get; set; }
        public int CustomersID { get; set; }
        public virtual Customers Customers { get; set; }
        public int CouponsToAvail { get; set; }
        public bool Status { get; set; } = true;
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
