namespace Ahmed_mart.Dtos.v1.ProductImagesDto
{
    public class GetProductImagesDto
    {
        public int ID { get; set; }
        public int StoreID { get; set; }
        public int ProductsID { get; set; }
        public string Path { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
