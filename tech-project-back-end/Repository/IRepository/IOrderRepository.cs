using tech_project_back_end.Models;
using tech_project_back_end.DTO.Order;
namespace tech_project_back_end.Repository.IRepository
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetList(DateTime startDate, DateTime endDate);
        
        Task<long> MonthRevenue(DateTime startDateOfMonth, DateTime endDateOfMonth);

        Task<long> DayRevenue(DateTime date);

        Task<int> Count();

        Task<Order> Create(Order order);

        Task<Order> UpdateState(string id, string state);

        Task<List<OrderDataTableDTO>> GetAll();

        Task<IEnumerable<dynamic>> GetAllWithDiscountOrderByDescending();

        Task<Order> FindById(string id);

        Task<IEnumerable<dynamic>> GetALlByIdOrderByDescending(string id);

        Task<IEnumerable<dynamic>> GetByUserId(string userId);

    }
}
