using Ahmed_mart.Dtos.v1.OrderDetailsDto;
using Ahmed_mart.Dtos.v1.OrderPaymentsDto;

namespace Ahmed_mart.Dtos.v1.OrdersDto
{
    public class UpdateOrdersDto
    {
        public int ID { get; set; }
        public int StoreID { get; set; }
       // public int CustomersId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal Tax { get; set; }
        public int OrdersStatusID { get; set; }
        public virtual UpdateOrderPaymentsDto OrderPayments { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public TimeSpan? DeliveryFrom { get; set; }
        public TimeSpan? DeliveryTill { get; set; }
        public byte DeliveryMode { get; set; }
        public decimal DeliveryCharges { get; set; }
        public decimal AdditionalCharges { get; set; }
        public string DeliveryAddress { get; set; }
        public string CustomerNotes { get; set; }
        public bool Status { get; set; }
        public byte OrderType { get; set; }
        public string RazorPayOrderId { get; set; }
        public string RazorPayStatus { get; set; }
        public string TransactionNumber { get; set; }
        public virtual ICollection<UpdateOrderDetailsDto> OrderDetails { get; set; } = new List<UpdateOrderDetailsDto>();
    }
}
