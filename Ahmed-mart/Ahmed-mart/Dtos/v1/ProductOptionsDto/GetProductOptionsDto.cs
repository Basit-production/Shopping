using Ahmed_mart.Dtos.v1.ProductDto;
using Ahmed_mart.Dtos.v1.ProductOptionDetailsDto;

namespace Ahmed_mart.Dtos.v1.ProductOptionsDto
{
    public class GetProductOptionsDto
    {
        public int ID { get; set; }
        public int StoreID { get; set; }
        public int ProductsID { get; set; }
        public virtual GetProductsDto Products { get; set; }
        public bool IsRequired { get; set; }
       // public int OptionsID { get; set; }
        //public virtual GetOptionsDto Options { get; set; }//need to Discuss
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public virtual ICollection<GetProductOptionDetailsDto> ProductOptionDetails { get; set; } = new List<GetProductOptionDetailsDto>();
    }
}
