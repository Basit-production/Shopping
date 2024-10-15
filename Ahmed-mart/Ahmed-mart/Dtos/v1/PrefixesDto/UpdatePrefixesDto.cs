

namespace Ahmed_mart.Dtos.v1.PrefixesDto
{
    public class UpdatePrefixesDto
    {
        public int ID { get; set; }
        public int StoreID { get; set; }
        public int TransactionType { get; set; }
        public int TransactionLength { get; set; }
        public string Prefix { get; set; }
        public int StartNumber { get; set; }
        public int CurrentNumber { get; set; }
        public bool Status { get; set; } = true;
        public bool IsDeleted { get; set; }
    }
}
