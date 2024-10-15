using System.ComponentModel.DataAnnotations;

namespace Ahmed_mart.Models.v1
{
    public class Orders:IEntityBase
    {
        public int ID { get; set; }
        public int StoreID { get; set; }
        //public int CustomersID { get; set; }
        //public virtual Customers Customers { get; set; }//need to Discuss
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal Tax { get; set; }
        public int OrdersStatusID { get; set; }
        public virtual OrdersStatus OrdersStatus { get; set; }
        public virtual OrderPayments OrderPayments { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public TimeSpan? DeliveryFrom { get; set; }
        public TimeSpan? DeliveryTill { get; set; }
        public byte DeliveryMode { get; set; }
        public decimal DeliveryCharges { get; set; }
        public decimal AdditionalCharges { get; set; }
        public string DeliveryAddress { get; set; }
        public string CustomerNotes { get; set; }
        public bool Status { get; set; } = true;
        public byte OrderType { get; set; }
        public string RazorPayOrderID { get; set; }
        public string RazorPayStatus { get; set; }
        public string TransactionNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
        public virtual ICollection<OrderHistory> OrderHistory { get; set; } = new List<OrderHistory>();
    }
}
