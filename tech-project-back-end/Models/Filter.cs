namespace tech_project_back_end.Models
{
    public class Filter
    {
        public string? SortBy { get; set; }
        public bool? IsDescending { get; set; } = false;
        public int? MinPrice { get; set; }
        public string? SearchKey { get; set; }
        public int? MaxPrice { get; set; }

        public string? CategoryId { get; set; }
        public string? SupplierId { get; set; }
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
    }
}
