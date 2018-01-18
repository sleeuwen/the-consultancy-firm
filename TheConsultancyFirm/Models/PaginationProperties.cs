namespace TheConsultancyFirm.Models
{
    public class PaginationProperties
    {
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
}
