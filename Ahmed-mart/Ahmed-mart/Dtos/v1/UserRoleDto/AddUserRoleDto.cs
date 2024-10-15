namespace Ahmed_mart.Dtos.v1.UserRoleDto
{
    public class AddUserRoleDto
    {
        public int StoreID { get; set; }
        public int RoleID { get; set; }
        public int PermissionModulesID { get; set; }
        public bool Access { get; set; }
        public bool Read { get; set; }
        public bool Write { get; set; }
        public bool FullAccess { get; set; }
        public int SortOrder { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
