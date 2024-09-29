namespace tech_project_back_end.DTO.RevenueDTO
{
    public class RevenueByDayDTO
    {
        public List<string> Days { get; set; } = new List<string>();
        public List<long> Revenues { get; set; } = new List<long>();
    }
}
