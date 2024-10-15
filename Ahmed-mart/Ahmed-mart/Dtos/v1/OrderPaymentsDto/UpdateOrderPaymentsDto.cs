namespace Ahmed_mart.Dtos.v1.OrderPaymentsDto
{
    public class UpdateOrderPaymentsDto
    {
        public int ID { get; set; }
        public int PaymentMode { get; set; }
        public decimal Amount { get; set; }
        public string RazorpayPaymentID { get; set; }
        public string RazorpayPaymentStatus { get; set; }
        public byte Status { get; set; }
    }
}
