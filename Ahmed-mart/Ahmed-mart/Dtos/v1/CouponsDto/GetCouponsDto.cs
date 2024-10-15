namespace Ahmed_mart.Dtos.v1.CouponsDto
{
    public class GetCouponsDto
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public string CouponCode { get; set; }
        public string CouponName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public float Discount { get; set; }
        public int UsePerCoupon { get; set; }
        public int UsePerCustomer { get; set; }
        public decimal CouponValidAbove { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
