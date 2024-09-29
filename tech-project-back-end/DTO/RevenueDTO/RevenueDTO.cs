namespace tech_project_back_end.DTO.RevenueDTO
{
    public class RevenueDTO
    {
        public decimal ThisMonthRevenue { get; set; }
        public decimal LastMonthRevenue { get; set; }
        public decimal PercentDifference { get; set; }
    }
}
