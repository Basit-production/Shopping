namespace Ahmed_mart.Dtos.v1.UserRoleDto
{
    public class UserPermissionsDto
    {
        public int StoreID { get; set; }
        public int RoleID { get; set; }
        public bool Access { get; set; }
        public bool Read { get; set; }
        public bool Write { get; set; }
        public bool FullAccess { get; set; }
    }
}
