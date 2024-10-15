namespace Ahmed_mart.Dtos.v1.GetOrderByStoreDtos
{
    public class GetOrderByStoreFilterDto
    {
        public int StoreID { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? OrderStatus { get; set; }
        public int? CustomerStatus { get; set; }
        public int? ProductsID { get; set; }
        public bool Default { get; set; }
    }
}
