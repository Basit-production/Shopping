namespace Ahmed_mart.Dtos.v1.PriceListDetailsDto
{
    public class AddPriceListDetailsDto
    {
        public int ProductID { get; set; }
        public decimal SalePrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public bool Status { get; set; } = true;
    }
}
