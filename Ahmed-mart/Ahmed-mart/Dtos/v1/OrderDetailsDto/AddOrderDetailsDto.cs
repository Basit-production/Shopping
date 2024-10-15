using Ahmed_mart.Dtos.v1.OrderOptionDetailsDto;

namespace Ahmed_mart.Dtos.v1.OrderDetailsDto
{
    public class AddOrderDetailsDto
    {
        public int? ProductsID { get; set; }
        public int? PriceListDetailsID { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal SalesPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal DiscountValue { get; set; }
        public virtual ICollection<AddOrderOptionDetailsDto> OrderOptionDetails { get; set; } = new List<AddOrderOptionDetailsDto>();
        public decimal Tax { get; set; }
        public decimal GrandTotal { get; set; }
        public bool Status { get; set; } = true;
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
