namespace Ahmed_mart.Models.v1
{
    public class Audit : IEntityBase
    {
        public int ID { get; set; }
        public string? HttpMethod { get; set; }
        public string? Url { get; set; }
        public string? EntityName { get; set; }
        public string? State { get; set; }
        public int EntityID { get; set; }
        public string? ReasonForChange { get; set; }
        public int RecordVersion { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public virtual ICollection<AuditDetails> AuditDetails { get; set; } = new List<AuditDetails>();
    }
}
