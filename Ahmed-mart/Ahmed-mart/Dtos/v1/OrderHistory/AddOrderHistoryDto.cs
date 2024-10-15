namespace Ahmed_mart.Dtos.v1.OrderHistoryDto
{
    public class AddOrderHistoryDto
    {
        public int OrdersID { get; set; }
        public int StoreID { get; set; }
        public string Comments { get; set; }
        public int OrdersStatusID { get; set; }
        public bool Status { get; set; } = true;
    }
}
