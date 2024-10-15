using System.ComponentModel.DataAnnotations;

namespace Ahmed_mart.Models.v1
{
    public class OrderHistory : IEntityBase
    {
        public int ID { get; set; }
        public int OrdersID { get; set; }
        public int StoreID { get; set; }
        public string Comments { get; set; }
        public int OrdersStatusID { get; set; }
        public virtual OrdersStatus OrdersStatus { get; set; }
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
