using Ahmed_mart.Dtos.v1.OrderDetailsDto;
using Ahmed_mart.Dtos.v1.ProductOptionDetailsDto;

namespace Ahmed_mart.Dtos.v1.OrderOptionDetailsDto
{
    public class GetOrderOptionDetailsDto
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public int OrderDetailsId { get; set; }
        public virtual GetOrderDetailsDto OrderDetails { get; set; }
        public int OptionsId { get; set; }
        public int? ProductOptionDetailsId { get; set; }
        public virtual GetProductOptionDetailsDto ProductOptionDetails { get; set; }
        public string Value { get; set; }
        public bool Status { get; set; } = true;
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
