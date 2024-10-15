using k8s.KubeConfigModels;

namespace Ahmed_mart.Models.v1
{
    public class UserTracker : IEntityBase
    {
        public int ID { get; set; }
        public int? AdminID { get; set; }
        public virtual Admin? Admin { get; set; }
        public int? UserID { get; set; }
        public virtual User? User { get; set; }
        public TimeSpan LogIn { get; set; }
        public TimeSpan LogOut { get; set; }
        public bool Status { get; set; } = true;
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
       // public byte[] RowVersion { get; set; }
    }
}
