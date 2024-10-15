using Ahmed_mart.Dtos.v1.ProductAttributesDto;
using Ahmed_mart.Dtos.v1.ProductImagesDto;
using Ahmed_mart.Dtos.v1.ProductOptionsDto;
using Ahmed_mart.Dtos.v1.RelatedProductsDto;

namespace Ahmed_mart.Dtos.v1.ProductDto
{
    public class AddProductsDto
    {
        public int StoreID { get; set; }
        //public int BrandID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal MRP { get; set; }
        public decimal Quantity { get; set; }
        public int MinimumQuantity { get; set; }
        public byte ProductFor { get; set; }
        public byte ProductType { get; set; }
        public string Description { get; set; }
        //public string Path { get; set; }
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
        public IFormFile File { get; set; }
        public virtual ICollection<AddProductOptionsDto> ProductOptions { get; set; } = new List<AddProductOptionsDto>();
        public virtual ICollection<AddProductAttributesDto> ProductAttributes { get; set; } = new List<AddProductAttributesDto>();
        public virtual ICollection<AddRelatedProductsDto> RelatedProducts { get; set; } = new List<AddRelatedProductsDto>();
        //public virtual ICollection<AddProductCategoriesDto> ProductCategories { get; set; } = new List<AddProductCategoriesDto>();
        public virtual ICollection<AddProductImagesDto> ProductImages { get; set; } = new List<AddProductImagesDto>();

        // filter fields
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? StockStatus { get; set; }
        public bool Default { get; set; }
        public int? ProductID { get; set; }
        public int? ProductCatID { get; set; }
    }
}
