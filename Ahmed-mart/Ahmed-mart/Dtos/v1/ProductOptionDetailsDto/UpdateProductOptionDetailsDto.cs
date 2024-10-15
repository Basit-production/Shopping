namespace Ahmed_mart.Dtos.v1.ProductOptionDetailsDto
{
    public class UpdateProductOptionDetailsDto
    {
        public int ID { get; set; }
        public string OptionValue { get; set; }
        public bool IsSubstractFromPrice { get; set; }
        public decimal Quantity { get; set; }
        public bool IsDeleted { get; set; }
        public decimal Price { get; set; }
    }
}
