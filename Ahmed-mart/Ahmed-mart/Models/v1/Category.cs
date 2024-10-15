using System.ComponentModel.DataAnnotations;

namespace Ahmed_mart.Models.v1
{
    public class Category :IEntityBase
    {
        public int ID { get; set; }
        //public int ProductsID { get; set; }
        public string? Path { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public int Discount { get; set; }//admin
        public bool AutoDiscount { get; set; } = false; //if AutoDiscount is true then Admin should able to give Discount 
        //public int? ParentCategoryID { get; set; }
        //[ForeignKey("ParentCategoryID")]
        public virtual Category ParentCategory { get; set; }
        public bool Status { get; set; } = true;
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
