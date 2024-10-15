namespace Ahmed_mart.Models.v1
{
    public interface IEntityBase
    {
        int ID { get; set; }
        bool IsDeleted { get; set; }
    }
}
