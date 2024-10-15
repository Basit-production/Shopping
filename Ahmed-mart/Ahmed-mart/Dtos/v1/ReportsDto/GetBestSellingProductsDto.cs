namespace Ahmed_mart.Dtos.v1.ReportsDto
{
    public class GetBestSellingProductsDto
    {
        public int ProductsID { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public string AdditionalCode { get; set; }
        public int Count { get; set; }
    }
}
