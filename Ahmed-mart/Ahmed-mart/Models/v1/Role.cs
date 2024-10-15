namespace Ahmed_mart.Models.v1
{
    public class Role : IEntityBase
    {
        public int ID { get; set; }
        public int? StoreID { get; set; }//Need to create
        public string Name { get; set; }
        public bool Status { get; set; } = true;
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        //public byte[] RowVersion { get; set; }//Should Be Added
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public bool IsDeleted { get; set; }
    }
}
