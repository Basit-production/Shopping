

using Ahmed_mart.Dtos.v1.PrefixesDto;
using Ahmed_mart.Dtos.v1.StoreTypeDto;
using Ahmed_mart.Dtos.v1.UserDtos;

namespace Ahmed_mart.Dtos.v1.StoreDto
{
    public class GetStoreDto
    {
        public int ID { get; set; }
        public Guid GuidId { get; set; }
        public string GSTIN { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int UserID { get; set; }
        public virtual GetUserDto User { get; set; }
        //public virtual GetStoreDeliveryDto StoreDelivery { get; set; }//need to create And Discuss
        //public virtual GetStoreSubscriptionDto StoreSubscription { get; set; } Need to Discuss And Create
        public string Path { get; set; }
        public bool Status { get; set; }
        public string StoreContact { get; set; }
        public bool IsPayment { get; set; }
        public bool PaymentPreference { get; set; }
        public int StoreTypeID { get; set; }
        public virtual GetStoreTypeDto StoreType { get; set; }//Need to create
        public string Key { get; set; }
        public string Secret { get; set; }
        public bool NumberPreference { get; set; }
        public bool IsFreeBirdModule { get; set; }
        public bool IsStore { get; set; }
        public bool IsBooking { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public virtual ICollection<GetPrefixesDto> PrefixDetails { get; set; } = new List<GetPrefixesDto>();
    }
}
