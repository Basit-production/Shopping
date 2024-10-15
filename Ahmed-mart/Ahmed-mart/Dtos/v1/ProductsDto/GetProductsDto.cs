using Ahmed_mart.Dtos.v1.ProductAttributesDto;
using Ahmed_mart.Dtos.v1.ProductImagesDto;
using Ahmed_mart.Dtos.v1.ProductOptionsDto;
using Ahmed_mart.Dtos.v1.RelatedProductsDto;

namespace Ahmed_mart.Dtos.v1.ProductDto
{
    public class GetProductsDto
    {
        public int ID { get; set; }
        public int StoreID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal MRP { get; set; }
        public decimal Quantity { get; set; }
        public int MinimumQuantity { get; set; }
        public byte ProductFor { get; set; }
        public byte ProductType { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal CostPrice { get; set; }
        public string SKU { get; set; }
        public string BarCode { get; set; }
        public bool IsTaxApplicable { get; set; }
        public decimal Tax { get; set; }
        public bool Status { get; set; }
        public int StockStatusId { get; set; }
        public bool IsShipping { get; set; }
        public bool IsProductDeductable { get; set; }
        public string TransactionNumber { get; set; }
        public bool IsDownloadable { get; set; }
        public bool RequestQuote { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual IList<GetProductOptionsDto> ProductOptions { get; set; } = new List<GetProductOptionsDto>();
        public virtual ICollection<GetProductAttributesDto> ProductAttributes { get; set; } = new List<GetProductAttributesDto>();
        public virtual ICollection<GetRelatedProductsDto> RelatedProducts { get; set; } = new List<GetRelatedProductsDto>();
        //public virtual ICollection<GetProductCategoriesDto> ProductCategories { get; set; } = new List<GetProductCategoriesDto>();
        public virtual ICollection<GetProductImagesDto> ProductImages { get; set; } = new List<GetProductImagesDto>();
    }
}
