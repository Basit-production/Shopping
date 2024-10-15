using Ahmed_mart.Dtos.v1.ProductAttributesDto;
using Ahmed_mart.Dtos.v1.ProductImagesDto;
using Ahmed_mart.Dtos.v1.ProductOptionsDto;
using Ahmed_mart.Dtos.v1.RelatedProductsDto;

namespace Ahmed_mart.Dtos.v1.ProductDto
{
    public class UpdateProductsDto
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
        public int StockStatusID { get; set; }
        public bool IsShipping { get; set; }
        public bool IsProductDeductable { get; set; }
        public string TransactionNumber { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public IFormFile File { get; set; }
        public virtual ICollection<UpdateProductOptionsDto> ProductOptions { get; set; } = new List<UpdateProductOptionsDto>();
        public virtual ICollection<UpdateProductAttributesDto> ProductAttributes { get; set; } = new List<UpdateProductAttributesDto>();
        public virtual ICollection<UpdateRelatedProductsDto> RelatedProducts { get; set; } = new List<UpdateRelatedProductsDto>();
        //public virtual ICollection<UpdateCategoriesDto> ProductCategories { get; set; } = new List<UpdateProductCategoriesDto>();
        public virtual ICollection<UpdateProductImagesDto> ProductImages { get; set; } = new List<UpdateProductImagesDto>();

    }
}
