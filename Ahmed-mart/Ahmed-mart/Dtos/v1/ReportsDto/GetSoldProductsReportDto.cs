namespace Ahmed_mart.Dtos.v1.ReportsDto
{
    public class GetSoldProductsReportDto
    {
        public int ProductsId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string AdditionalCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Total { get; set; }
    }
}
