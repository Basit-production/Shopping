using System.ComponentModel.DataAnnotations;

namespace Ahmed_mart.Models.v1
{
    public class PriceListDetails :IEntityBase
    {
        public int ID { get; set; }
        public int PriceListID { get; set; }
        public virtual PriceList PriceList { get; set; }
        public int? ProductID { get; set; }
        public virtual Products Product { get; set; }
        public decimal SalePrice { get; set; }
        public decimal PurchasePrice { get; set; }
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
