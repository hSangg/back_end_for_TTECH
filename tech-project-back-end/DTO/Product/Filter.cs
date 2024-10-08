﻿namespace tech_project_back_end.DTO
{
    public class Filter
    {
        public string SortBy { get; set; } = "created_date";
        public bool IsDescending { get; set; } = true;
        public int MinPrice { get; set; } = 0;
        public string SearchKey { get; set; } = "";
        public int MaxPrice { get; set; } = 999999999;
        public string? CategoryId { get; set; }
        public string? SupplierId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }
}
