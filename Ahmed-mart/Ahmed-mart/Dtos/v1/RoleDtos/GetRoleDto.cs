using Ahmed_mart.Dtos.v1.UserRoleDto;

namespace Ahmed_mart.Dtos.v1.RoleDtos
{
    public class GetRoleDto
    {
        public int ID { get; set; }
        public int? StoreID { get; set; }// need to discuss
        public string Name { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public virtual ICollection<GetUserRoleDto> UserRoles { get; set; } = new List<GetUserRoleDto>();
    }
}
