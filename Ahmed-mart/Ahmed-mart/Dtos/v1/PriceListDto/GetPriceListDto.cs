using Ahmed_mart.Dtos.v1.PriceListDetailsDto;
using Ahmed_mart.Dtos.v1.StoreDto;

namespace Ahmed_mart.Dtos.v1.PriceListDto
{
    public class GetPriceListDto
    {
        public int ID { get; set; }
        public int StoreID { get; set; }
        public virtual GetStoreDto Store { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public virtual ICollection<GetPriceListDetailsDto> PriceListDetails { get; set; } = new List<GetPriceListDetailsDto>();
    }
}
