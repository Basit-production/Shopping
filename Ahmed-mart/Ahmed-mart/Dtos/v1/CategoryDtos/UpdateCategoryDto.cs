namespace Ahmed_mart.Dtos.v1.CategoryDtos
{
    public class UpdateCategoryDto
    {
        public int ID { get; set; }
        public string? CategoryCode { get; set; }
        public string? CategoryName { get; set; }
        //public int? ParentCategoryID { get; set; }
        public bool Status { get; set; } = true;
        public int Discount { get; set; }//admin 
        public bool AutoDiscount { get; set; } = false;
        public bool IsDeleted { get; set; } 
        public IFormFile? File { get; set; }
    }
}
