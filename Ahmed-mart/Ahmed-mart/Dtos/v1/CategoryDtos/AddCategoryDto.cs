namespace Ahmed_mart.Dtos.v1.CategoryDtos
{
    public class AddCategoryDto
    {
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        //public int? ParentCategoryID { get; set; }
        public int Discount { get; set; }//admin 
        public bool AutoDiscount { get; set; } = false;
        public IFormFile? File { get; set; }
    }
}
