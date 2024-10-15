namespace SCare.Api.Dtos.PriceListDetailsDto
{
    public class UpdatePriceListDetailsDto
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        public decimal SalePrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public bool Status { get; set; }
    }
}
