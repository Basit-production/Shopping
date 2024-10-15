namespace Ahmed_mart.Dtos.v1.ProductOptionDetailsDto
{
    public class AddProductOptionDetailsDto
    {
        public int StoreID { get; set; }
        public int ProductOptionsID { get; set; }
        public string OptionValue { get; set; }
        public decimal Quantity { get; set; }
        public bool IsSubstractFromPrice { get; set; }
        public decimal Price { get; set; }
        public bool Status { get; set; } = true;
    }
}
