namespace Ahmed_mart.Dtos.v1.ProductImagesDto
{
    public class AddProductImagesDto
    {
        public int StoreID { get; set; }
        public int ProductsID { get; set; }
        //public string Path { get; set; }
        public bool Status { get; set; }
        public IFormFile File { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
