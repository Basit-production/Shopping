using Ahmed_mart.Dtos.v1.CouponsDto;
using Ahmed_mart.Dtos.v1.CustomersDto;

namespace Ahmed_mart.Dtos.v1.CustomerCouponsDto
{
    public class UpdateCustomerCouponsDto
    {
        public int ID{ get; set; }
        public int CouponsID{ get; set; }
        public virtual UpdateCouponsDto Coupons { get; set; }
        public int CustomersId { get; set; }
        public virtual UpdateCustomersDto Customers { get; set; }
        public int CouponsToAvail { get; set; }
        public bool Status { get; set; } = true;
    }
}
