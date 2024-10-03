namespace tech_project_back_end.DTO
{
    public class FilteredProductResponse
    {
        public List<ProductDTO> Products { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalProducts { get; set; }
    }

}
