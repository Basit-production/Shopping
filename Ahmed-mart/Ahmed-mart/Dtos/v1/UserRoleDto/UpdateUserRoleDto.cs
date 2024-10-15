namespace Ahmed_mart.Dtos.v1.UserRoleDto
{
    public class UpdateUserRoleDto
    {
        public int ID { get; set; }
        public int StoreID { get; set; }
        public int RoleID { get; set; }
        public int PermissionModulesID { get; set; }//Need To Discuss
        public bool Access { get; set; }
        public bool Read { get; set; }
        public bool Write { get; set; }
        public bool FullAccess { get; set; }
        public int SortOrder { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
