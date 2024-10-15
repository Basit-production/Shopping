using System.ComponentModel.DataAnnotations;

namespace Ahmed_mart.Models.v1
{
    public class ProductOptions :IEntityBase
    {
        public int ID { get; set; }
        public int StoreID { get; set; }
        public int ProductsID { get; set; }
        public int OptionsID { get; set; }
        public bool IsRequired { get; set; }
        public bool Status { get; set; } = true;
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual ICollection<ProductOptionDetails> ProductOptionDetails { get; set; } = new List<ProductOptionDetails>();
    }
}
