using Ahmed_mart.Dtos.v1.CouponsDto;
using Ahmed_mart.Dtos.v1.CustomersDto;

namespace Ahmed_mart.Dtos.v1.CustomerCouponsDto
{
    public class AddCustomerCouponsDto
    {
        public int CouponsID{ get; set; }
        public virtual AddCouponsDto Coupons { get; set; }
        public int CustomersID{ get; set; }
        public virtual AddCustomersDto Customers { get; set; }
        public int CouponsToAvail { get; set; }
        public bool Status { get; set; } = true;
    }
}
