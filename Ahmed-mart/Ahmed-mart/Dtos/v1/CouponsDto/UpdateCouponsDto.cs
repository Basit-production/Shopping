namespace Ahmed_mart.Dtos.v1.CouponsDto
{
    public class UpdateCouponsDto
    {
        public int ID { get; set; }
        public int StoreID { get; set; }
        public string CouponCode { get; set; }
        public string CouponName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public float Discount { get; set; }
        public int UsePerCoupon { get; set; }
        public int UsePerCustomer { get; set; }
        public decimal CouponValidAbove { get; set; }
        public bool Status { get; set; }
    }
}
