using Ahmed_mart.Dtos.v1.OrderStatusDto;

namespace Ahmed_mart.Dtos.v1.OrderHistoryDto
{
    public class GetOrderHistoryDto
    {
        public int ID { get; set; }
        public int OrdersID { get; set; }
        public int StoreID { get; set; }
        public string Comments { get; set; }
        public int OrdersStatusID { get; set; }
        //public virtual OrdersStatus OrdersStatus { get; set; }
        public virtual GetOrderStatusDto OrdersStatus { get; set; }
        public bool Status { get; set; } = true;
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
