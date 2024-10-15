using Ahmed_mart.Dtos.v1.OrderDetailsDto;
using Ahmed_mart.Dtos.v1.OrderHistoryDto;
using Ahmed_mart.Dtos.v1.OrderPaymentsDto;

namespace Ahmed_mart.Dtos.v1.OrdersDto
{
    public class AddOrdersDto
    {
        public int StoreID { get; set; }
       // public int? CustomersID { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal Tax { get; set; }
        public int OrdersStatusID { get; set; }
        public virtual AddOrderPaymentsDto OrderPayments { get; set; }
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
        public string RazorPayOrderId { get; set; }
        public string RazorPayStatus { get; set; }
        public int CouponID { get; set; }
        public string TransactionNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public virtual ICollection<AddOrderDetailsDto> OrderDetails { get; set; } = new List<AddOrderDetailsDto>();
        public virtual ICollection<AddOrderHistoryDto> OrderHistory { get; set; } = new List<AddOrderHistoryDto>();

        // filter fields
        //public DateTime? FromDate { get; set; }
        //public DateTime? ToDate { get; set; }
        //public int? OrderStatus { get; set; }
        //public int? CustomerStatus { get; set; }
        //public int? ProductsID { get; set; }
        //public bool Default { get; set; }
    }
}
