using Ahmed_mart.Dtos.v1.OrderDetailsDto;
using Ahmed_mart.Dtos.v1.OrderHistoryDto;
using Ahmed_mart.Dtos.v1.OrderPaymentsDto;
using Ahmed_mart.Dtos.v1.OrderStatusDto;

namespace Ahmed_mart.Dtos.v1.OrdersDto
{
    public class GetOrdersDto
    {
        public int ID { get; set; }
        public int StoreID { get; set; }
        //public int CustomersId { get; set; }
        //public virtual GetCustomersDto Customers { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal Tax { get; set; }
        public int OrdersStatusId { get; set; }
        public virtual GetOrderStatusDto OrdersStatus { get; set; }
        public virtual GetOrderPaymentsDto OrderPayments { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public TimeSpan? DeliveryFrom { get; set; }
        public TimeSpan? DeliveryTill { get; set; }
        public byte DeliveryMode { get; set; }
        public decimal DeliveryCharges { get; set; }
        public decimal AdditionalCharges { get; set; }
        public string DeliveryAddress { get; set; }
        public string CustomerNotes { get; set; }
        public bool Status { get; set; }
        public string TransactionNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public byte OrderType { get; set; }
        public string RazorPayOrderId { get; set; }
        public string RazorPayStatus { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public virtual ICollection<GetOrderDetailsDto> OrderDetails { get; set; } = new List<GetOrderDetailsDto>();
        public virtual ICollection<GetOrderHistoryDto> OrderHistory { get; set; } = new List<GetOrderHistoryDto>();
    }
}
