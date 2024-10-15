using Ahmed_mart.Dtos.v1.AttributesDto;
using Ahmed_mart.Dtos.v1.ProductDto;

namespace Ahmed_mart.Dtos.v1.ProductAttributesDto
{
    public class GetProductAttributesDto
    {
        public int ID { get; set; }
        public int StoreID { get; set; }
        public int ProductsID { get; set; }
        public virtual GetProductsDto Products { get; set; }
        public int AttributesID { get; set; }
        public virtual GetAttributesDto Attributes { get; set; }
        public string Value { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
