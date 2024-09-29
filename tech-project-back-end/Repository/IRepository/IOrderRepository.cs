using tech_project_back_end.Models;

namespace tech_project_back_end.Repository.IRepository
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetList(DateTime startDate, DateTime endDate);
        Task<long> MonthRevenue(DateTime startDateOfMonth, DateTime endDateOfMonth);
        Task<long> DayRevenue(DateTime date);
        Task<int> Count();
    }
}
