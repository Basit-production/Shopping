using Ahmed_mart.Dtos.v1.ProductDto;

namespace Ahmed_mart.Dtos.v1.RelatedProductsDto
{
    public class GetRelatedProductsDto
    {
        public int ID { get; set; }
        public int StoreID { get; set; }
        public int ProductsID { get; set; }
        public virtual GetProductsDto Products { get; set; }
        public int? RelatedProductID { get; set; }
        public virtual GetProductsDto RelatedProduct { get; set; }
        public bool Status { get; set; } = true;
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
