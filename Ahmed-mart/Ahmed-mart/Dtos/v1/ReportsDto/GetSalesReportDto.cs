namespace Ahmed_mart.Dtos.v1.ReportsDto
{
    public class GetSalesReportDto
    {
        public decimal GrandTotal { get; set; }
        public decimal Tax { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
        public int? Status { get; set; }
    }
}
