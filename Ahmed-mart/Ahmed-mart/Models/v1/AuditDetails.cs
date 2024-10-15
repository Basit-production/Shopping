namespace Ahmed_mart.Models.v1
{
    public class AuditDetails: IEntityBase
    {
        public int ID { get; set; }
        public int AuditID { get; set; }
        public virtual Audit Audit { get; set; }
        public string? PropertyName { get; set; }
        public string? OriginalValue { get; set; }
        public string? CurrentValue { get; set; }
        public bool IsDeleted { get; set; }
    }
}
