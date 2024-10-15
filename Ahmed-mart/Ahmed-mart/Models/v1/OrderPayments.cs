using System.ComponentModel.DataAnnotations;

namespace Ahmed_mart.Models.v1
{
    public class OrderPayments : IEntityBase
    {
        public int ID { get; set; }
        public int OrdersID { get; set; }
        public virtual Orders Orders { get; set; }
        public int PaymentMode { get; set; }
        public decimal Amount { get; set; }
        public string RazorpayPaymentID { get; set; }
        public string RazorpayPaymentStatus { get; set; }
        public byte Status { get; set; } = 1;
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
