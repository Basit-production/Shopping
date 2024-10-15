using Ahmed_mart.Dtos.v1.ProductDto;

namespace Ahmed_mart.Dtos.v1.PriceListDetailsDto
{
    public class GetPriceListDetailsDto
    {
        public int Id { get; set; }
        public int PriceListId { get; set; }
        public int ProductId { get; set; }
        public virtual GetProductsDto Product { get; set; }
        public decimal SalePrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
