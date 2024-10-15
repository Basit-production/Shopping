

using Ahmed_mart.Dtos.v1.PrefixesDto;
using Ahmed_mart.Dtos.v1.StoreTypeDto;

namespace Ahmed_mart.Dtos.v1.StoreDto
{
    public class UpdateStoreDto
    {
        public int ID { get; set; }
        public string GSTIN { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int UserID { get; set; }
        //public virtual UpdateStoreDeliveryDto StoreDelivery { get; set; }//Need to crete And Discuss
        //public virtual UpdateStoreSubscriptionDto StoreSubscription { get; set; } Need to Discuss And Create
        public IFormFile? File { get; set; }
        public bool Status { get; set; } = true;
        public string StoreContact { get; set; }
        public int StoreTypeID { get; set; }
        //public virtual UpdateStoreTypeDto StoreType { get; set; }
        public bool IsPayment { get; set; }
        public bool IsStore { get; set; }
        public bool IsBooking { get; set; }
        public bool PaymentPreference { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }
        public bool NumberPreference { get; set; }
        public bool IsFreeBirdModule { get; set; }
        public virtual ICollection<UpdatePrefixesDto> PrefixDetails { get; set; } = new List<UpdatePrefixesDto>();
    }
}
