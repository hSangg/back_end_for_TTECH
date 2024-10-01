namespace tech_project_back_end.DTO
{
    public class CategoryDTO
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
