using tech_project_back_end.Data;
using tech_project_back_end.Repository.IRepository;
using tech_project_back_end.Models;
using Microsoft.EntityFrameworkCore;

namespace tech_project_back_end.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(AppDbContext appDbContext, ILogger<OrderRepository> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public async Task<List<Order>> GetList(DateTime startDate, DateTime endDate)
        {
            return await _appDbContext.Order
                .Where(o => o.createdAt >= startDate && o.createdAt <= endDate)
                .ToListAsync();
        }

        public async Task<long> MonthRevenue(DateTime startDateOfMonth, DateTime endDateOfMonth)
        {
            return await _appDbContext.Order
                 .Where(o => o.createdAt >= startDateOfMonth && o.createdAt <= endDateOfMonth)
                 .SumAsync(o => o.total);
        }

        public async Task<long> DayRevenue(DateTime date)
        {
            return await _appDbContext.Order
                 .Where(o => o.createdAt.Date == date.Date)
                 .SumAsync(o => o.total);
        }

        public async Task<int> Count() { return await _appDbContext.Order.CountAsync(); }
    }
}
