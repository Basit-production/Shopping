using SCare.Api.Dtos.PriceListDetailsDto;

namespace Ahmed_mart.Dtos.v1.PriceListDto
{
    public class UpdatePriceListDto
    {
        public int ID { get; set; }
        public int StoreID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        public virtual ICollection<UpdatePriceListDetailsDto> PriceListDetails { get; set; } = new List<UpdatePriceListDetailsDto>();
    }
}
