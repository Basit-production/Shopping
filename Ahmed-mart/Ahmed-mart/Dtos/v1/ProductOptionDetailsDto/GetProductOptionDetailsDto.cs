using Ahmed_mart.Dtos.v1.ProductOptionsDto;

namespace Ahmed_mart.Dtos.v1.ProductOptionDetailsDto
{
    public class GetProductOptionDetailsDto
    {
        public int Id { get; set; }
        public int StoreID { get; set; }
        public int ProductOptionsID { get; set; }
        public virtual GetProductOptionsDto ProductOptions { get; set; }
        public string OptionValue { get; set; }
        public decimal Quantity { get; set; }
        public bool IsSubstractFromPrice { get; set; }
        public decimal Price { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
