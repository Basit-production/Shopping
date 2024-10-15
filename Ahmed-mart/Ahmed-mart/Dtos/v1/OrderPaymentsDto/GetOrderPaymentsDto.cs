namespace Ahmed_mart.Dtos.v1.OrderPaymentsDto
{
    public class GetOrderPaymentsDto
    {
        public int ID { get; set; }
        public int OrdersID { get; set; }
        public int PaymentMode { get; set; }
        public decimal Amount { get; set; }
        public string RazorpayPaymentID { get; set; }
        public string RazorpayPaymentStatus { get; set; }
        public byte Status { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
