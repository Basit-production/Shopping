namespace Ahmed_mart.Dtos.v1.OrderOptionDetailsDto
{
    public class AddOrderOptionDetailsDto
    {
        public int StoreID { get; set; }
        //public int OptionsId { get; set; }
        public int? ProductOptionDetailsID { get; set; }
        public string Value { get; set; }
        public bool Status { get; set; } = true;
    }
}
