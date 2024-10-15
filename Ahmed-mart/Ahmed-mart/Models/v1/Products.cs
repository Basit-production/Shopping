using System.ComponentModel.DataAnnotations;

namespace Ahmed_mart.Models.v1
{
    public class Products:IEntityBase
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
        public bool Status { get; set; } = true;
        public int StockStatusID { get; set; }
        public virtual StockStatus StockStatus { get; set; }//Need to create
        public bool IsShipping { get; set; }
        public bool IsProductDeductable { get; set; }
        public string TransactionNumber { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        //public virtual ICollection<ProductCategories> ProductCategories { get; set; } = new List<ProductCategories>();
        public virtual ICollection<ProductOptions> ProductOptions { get; set; } = new List<ProductOptions>();
        public virtual ICollection<ProductAttributes> ProductAttributes { get; set; } = new List<ProductAttributes>();
        public virtual ICollection<RelatedProducts> RelatedProducts { get; set; } = new List<RelatedProducts>();
        public virtual ICollection<ProductImages> ProductImages { get; set; } = new List<ProductImages>();
    }
}
