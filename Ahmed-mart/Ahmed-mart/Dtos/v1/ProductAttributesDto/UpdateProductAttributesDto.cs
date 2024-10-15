namespace Ahmed_mart.Dtos.v1.ProductAttributesDto
{
    public class UpdateProductAttributesDto
    {
        public int ID { get; set; }
        public int AttributesID { get; set; }
        public bool IsDeleted { get; set; }
        public string Value { get; set; }
        public int MdifiedBy { get; set; }
        public int MdifiedAt { get; set; }
    }
}
