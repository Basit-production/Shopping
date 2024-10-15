namespace Ahmed_mart.Models.v1
{
    public class SmtpDetails : IEntityBase
    {
        public int ID { get; set; }
        public int? StoreID { get; set; }
        //public virtual Store Store { get; set; }//Need To Be Discuss
        public string From { get; set; }
        public string DisplayName { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Status { get; set; } = true;
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
