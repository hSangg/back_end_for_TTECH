namespace tech_project_back_end.DTO
{
    public class RevenueByYearDTO
    {
        public List<string> Labels { get; set; }
        public List<long> Revenues { get; set; }
        public RevenueByYearDTO()
        {
            Labels = new List<string>();
            Revenues = new List<long>();
        }
    }
}
