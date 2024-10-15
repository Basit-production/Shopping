namespace Ahmed_mart.Dtos.v1.ReportsDto
{
    public class GetCustomerSalesReportDto
    {
        public int CustomersId { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }
        public int CustomerGroupId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Status { get; set; }
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
    }
}
