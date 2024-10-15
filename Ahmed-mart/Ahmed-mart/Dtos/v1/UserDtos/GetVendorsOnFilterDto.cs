namespace Ahmed_mart.Dtos.v1.UserDtos
{
    public class GetVendorsOnFilterDto
    {
        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public bool? Status { get; set; }
        public virtual IEnumerable<GetUserDto> Users { get; set; }
    }
}
