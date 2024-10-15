namespace Ahmed_mart.Dtos.v1.OrderDetailsDto
{
    public class UpdateOrderDetailsDto
    {
        public int ID { get; set; }
        public int? ProductsID { get; set; }
        public int? PriceListDetailsID { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal SalesPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal Tax { get; set; }
        public decimal GrandTotal { get; set; }
        public bool Status { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
