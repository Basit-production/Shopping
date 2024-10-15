using Ahmed_mart.Dtos.v1.ProductOptionDetailsDto;

namespace Ahmed_mart.Dtos.v1.ProductOptionsDto
{
    public class AddProductOptionsDto
    {
        public int StoreID { get; set; }
        public int ProductsID { get; set; }
        public bool IsRequired { get; set; }
        public int OptionsID { get; set; }
        public bool Status { get; set; } = true;
        public virtual ICollection<AddProductOptionDetailsDto> ProductOptionDetails { get; set; } = new List<AddProductOptionDetailsDto>();
    }
}
