namespace Ahmed_mart.Dtos.v1.OrderStatusDto
{
    public class GetOrderStatusDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
