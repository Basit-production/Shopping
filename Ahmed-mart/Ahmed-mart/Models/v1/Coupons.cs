using System.ComponentModel.DataAnnotations;
using System.Timers;

namespace Ahmed_mart.Models.v1
{
    public class Coupons :IEntityBase
    {
        public int ID { get; set; }
        public int StoreID { get; set; }
        public virtual Store Store { get; set; }
        public string CouponCode { get; set; }
        public string CouponName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public float Discount { get; set; }
        public int UsePerCoupon { get; set; }
        public int UsePerCustomer { get; set; }
        public decimal CouponValidAbove { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
