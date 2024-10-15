namespace Ahmed_mart.Dtos.v1.CategoryDtos
{
    public class GetCategoryDto
    {
        public int ID { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public virtual GetCategoryDto ParentCategory { get; set; }
        public int Discount { get; set; }//Admin
        public bool Status { get; set; }
        public string? Path { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
