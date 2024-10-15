using System.ComponentModel.DataAnnotations;

namespace Ahmed_mart.Models.v1
{
    public class ProductOptionDetails :IEntityBase
    {
        public int ID { get; set; }
        public int StoreID { get; set; }
        public int ProductOptionsID { get; set; }
        public string OptionValue { get; set; }
        public decimal Quantity { get; set; }
        public bool IsSubstractFromPrice { get; set; }
        public decimal Price { get; set; }
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
