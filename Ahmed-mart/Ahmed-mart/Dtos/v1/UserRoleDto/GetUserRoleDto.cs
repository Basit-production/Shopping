using Ahmed_mart.Dtos.v1.RoleDtos;
using Ahmed_mart.Models.v1;

namespace Ahmed_mart.Dtos.v1.UserRoleDto
{
    public class GetUserRoleDto
    {
        public int ID { get; set; }
        public int StoreID { get; set; }
        public int RoleID { get; set; }
        public virtual GetRoleDto Role { get; set; }
        //public int PermissionModulesId { get; set; }//Need to Discuss
       // public virtual PermissionModules Permission { get; set; }
        public bool Access { get; set; }
        public bool Read { get; set; }
        public bool Write { get; set; }
        public bool FullAccess { get; set; }
        public int SortOrder { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
       // public byte[] RowVersion { get; set; }
    }
}
