using Ahmed_mart.Dtos.v1.OrderOptionDetailsDto;
using Ahmed_mart.Dtos.v1.PriceListDetailsDto;
using Ahmed_mart.Dtos.v1.ProductDto;

namespace Ahmed_mart.Dtos.v1.OrderDetailsDto
{
    public class GetOrderDetailsDto
    {
        public int ID { get; set; }
        public int OrdersID { get; set; }
        public int? ProductsID { get; set; }
        public virtual GetProductsDto Products { get; set; }
        public int? PriceListDetailsID { get; set; }
        public virtual GetPriceListDetailsDto PriceListDetails { get; set; }//need to create
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal SalesPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal Tax { get; set; }
        public decimal GrandTotal { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public byte[] RowVersion { get; set; }
        public virtual ICollection<GetOrderOptionDetailsDto> OrderOptionDetails { get; set; } = new List<GetOrderOptionDetailsDto>();
    }
}
