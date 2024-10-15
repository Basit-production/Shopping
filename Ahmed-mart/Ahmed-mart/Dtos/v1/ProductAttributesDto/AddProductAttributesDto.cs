namespace Ahmed_mart.Dtos.v1.ProductAttributesDto
{
    public class AddProductAttributesDto
    {
        public int ID { get; set; }
        public int StoreID { get; set; }
        public int ProductsID { get; set; }
        public int AttributesID { get; set; }
        public string Value { get; set; }
        public bool Status { get; set; }
    }
}
