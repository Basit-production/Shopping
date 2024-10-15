
namespace Ahmed_mart.Dtos.v1.StoreTypeDto
{
    public class UpdateStoreTypeDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
