using Ahmed_mart.Dtos.v1.CouponsDto;
using Ahmed_mart.Dtos.v1.CustomersDto;

namespace Ahmed_mart.Dtos.v1.CustomerCouponsDto
{
    public class GetCustomerCouponsDto
    {
        public int Id { get; set; }
        public int CouponsId { get; set; }
        public virtual GetCouponsDto Coupons { get; set; }
        public int CustomersId { get; set; }
        public virtual GetCustomersDto Customers { get; set; }
        public int CouponsToAvail { get; set; }
        public bool Status { get; set; } = true;
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
