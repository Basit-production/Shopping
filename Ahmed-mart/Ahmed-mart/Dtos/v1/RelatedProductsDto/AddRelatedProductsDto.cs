namespace Ahmed_mart.Dtos.v1.RelatedProductsDto
{
    public class AddRelatedProductsDto
    {
        public int StoreID { get; set; }
        public int ProductsID { get; set; }
        public int? RelatedProductID { get; set; }
        public bool Status { get; set; }
    }
}
