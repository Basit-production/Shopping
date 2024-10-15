
namespace Ahmed_mart.Dtos.v1.StoreTypeDto
{
    public class AddStoreTypeDto
    {
        public string Name { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
