using System.ComponentModel.DataAnnotations;

namespace Ahmed_mart.Models.v1
{
    public class OrderDetails : IEntityBase
    {
        public int ID { get; set; }
        public int OrdersID { get; set; }
        public virtual Orders Orders { get; set; }
        public int? ProductsID { get; set; }
        public virtual Products Products { get; set; }
        public int? PriceListDetailsID { get; set; }
        public virtual PriceListDetails PriceListDetails { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal SalesPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal Tax { get; set; }
        public decimal GrandTotal { get; set; }
        public bool Status { get; set; } = true;
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public virtual ICollection<OrderOptionDetails> OrderOptionDetails { get; set; } = new List<OrderOptionDetails>();//need to create
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
