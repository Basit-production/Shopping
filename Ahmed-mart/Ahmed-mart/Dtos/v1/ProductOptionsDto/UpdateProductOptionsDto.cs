using Ahmed_mart.Dtos.v1.ProductOptionDetailsDto;
namespace Ahmed_mart.Dtos.v1.ProductOptionsDto
{
    public class UpdateProductOptionsDto
    {
        public int ID { get; set; }
        public bool IsRequired { get; set; }
        public bool IsDeleted { get; set; }
        public int OptionsId { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
        public virtual ICollection<UpdateProductOptionDetailsDto> ProductOptionDetails { get; set; } = new List<UpdateProductOptionDetailsDto>();
    }
}