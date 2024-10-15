
using Ahmed_mart.Dtos.v1.PrefixesDto;
using Ahmed_mart.Dtos.v1.StoreTypeDto;
namespace Ahmed_mart.Dtos.v1.StoreDto
{
    public class AddStoreDto
    {
        public string GSTIN { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int UserID { get; set; }
        //public virtual AddStoreDeliveryDto StoreDelivery { get; set; }//Need to Discuss Create
        //public virtual AddStoreSubscriptionDto StoreSubscription { get; set; } Need to Discuss Create
        public IFormFile? File { get; set; }
        public bool Status { get; set; } = true;
        public string StoreContact { get; set; }
        public int StoreTypeID { get; set; }
        //public virtual AddStoreTypeDto StoreType { get; set; }
        public bool IsPayment { get; set; }
        public bool IsStore { get; set; }
        public bool IsBooking { get; set; }
        public bool PaymentPreference { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }
        public bool NumberPreference { get; set; }
        public bool IsFreeBirdModule { get; set; }
        //public virtual ICollection<AddPrefixesDto> PrefixDetails { get; set; } = new List<AddPrefixesDto>();
    }
}
