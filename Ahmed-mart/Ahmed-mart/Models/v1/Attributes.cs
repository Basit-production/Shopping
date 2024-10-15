using System.ComponentModel.DataAnnotations;

namespace Ahmed_mart.Models.v1
{
    public class Attributes :IEntityBase
    {
        public int ID { get; set; }
        public int StoreID { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
