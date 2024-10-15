using System.ComponentModel.DataAnnotations;
using System.Timers;

namespace Ahmed_mart.Models.v1
{
    public class Prefixes :IEntityBase
    {
        public int ID { get; set; }
        public int StoreID { get; set; }
        public virtual Store Store { get; set; }
        public int TransactionType { get; set; }
        public int TransactionLength { get; set; }
        public string Prefix { get; set; }
        public int StartNumber { get; set; }
        public int CurrentNumber { get; set; }
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
