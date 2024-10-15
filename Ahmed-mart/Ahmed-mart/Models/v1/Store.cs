
using System.ComponentModel.DataAnnotations;

namespace Ahmed_mart.Models.v1
{
    public class Store:IEntityBase
    {
        public int ID { get; set; }
        public Guid GuidId { get; set; } = Guid.NewGuid();
        public string GSTIN { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Path { get; set; }
        public int UserID { get; set; }
        public virtual User User { get; set; }
        //public virtual StoreDelivery StoreDelivery { get; set; }  //need to Create
        //public virtual StoreSubscription StoreSubscription { get; set; } //Need To Discuss And Create
        public bool NumberPreference { get; set; }
        public bool IsFreeBirdModule { get; set; }
        public bool Status { get; set; } = true;
        public string StoreContact { get; set; }
        public int StoreTypeID { get; set; }
        public virtual StoreType StoreType { get; set; }
        public bool IsPayment { get; set; }
        public bool PaymentPreference { get; set; }
        public bool IsStore { get; set; }
        public bool IsBooking { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual ICollection<Prefixes> PrefixDetails { get; set; } = new List<Prefixes>();
    }
}
