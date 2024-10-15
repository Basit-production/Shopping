namespace Ahmed_mart.Dtos.v1.ProductDto
{
    public class GetStoreProductsDto
    {
        public int StoreID { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public string Search { get; set; }
        public virtual IEnumerable<GetProductsDto> Products { get; set; }
    }
}
