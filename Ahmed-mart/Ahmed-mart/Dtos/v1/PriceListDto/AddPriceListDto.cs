using Ahmed_mart.Dtos.v1.PriceListDetailsDto;

namespace Ahmed_mart.Dtos.v1.PriceListDto
{
    public class AddPriceListDto
    {
        public int StoreID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; } = true;
        public virtual ICollection<AddPriceListDetailsDto> PriceListDetails { get; set; } = new List<AddPriceListDetailsDto>();
    }
}
