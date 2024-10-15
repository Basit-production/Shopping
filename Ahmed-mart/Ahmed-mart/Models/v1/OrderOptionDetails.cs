using System.ComponentModel.DataAnnotations;

namespace Ahmed_mart.Models.v1
{
    public class OrderOptionDetails : IEntityBase
    {
        public int ID { get; set; }
        public int StoreId { get; set; }
        public int OrderDetailsID { get; set; }
        //public int? OptionsID { get; set; }
        public int? ProductOptionDetailsID { get; set; }
        public virtual ProductOptionDetails ProductOptionDetails { get; set; }
        public string Value { get; set; }
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
