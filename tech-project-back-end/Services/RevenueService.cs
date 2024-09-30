using tech_project_back_end.DTO;
using tech_project_back_end.DTO.RevenueDTO;
using tech_project_back_end.Repository.IRepository;
using tech_project_back_end.Services.IService;

namespace tech_project_back_end.Services
{
    public class RevenueService : IRevenueService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<RevenueService> _logger;

        public RevenueService(IOrderRepository orderRepository, ILogger<RevenueService> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<RevenueDTO> GetRevenue()
        {
            DateTime currentMonthStart = DateTime.Now.AddDays(-1 * DateTime.Now.Day + 1).Date;
            DateTime currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1);
            DateTime lastMonthStart = currentMonthStart.AddMonths(-1).AddDays(-1 * DateTime.Now.Day + 1).Date;
            DateTime lastMonthEnd = lastMonthStart.AddMonths(1).AddDays(-1);

            var ordersThisMonth = await _orderRepository.GetList(currentMonthStart, currentMonthEnd);
            var ordersLastMonth = await _orderRepository.GetList(lastMonthStart, lastMonthEnd);

            decimal thisMonthRevenue = ordersThisMonth.Sum(o => o.Total - o.DeliveryFee);
            decimal lastMonthRevenue = ordersLastMonth.Sum(o => o.Total - o.DeliveryFee);
            decimal percentDifference;

            if (lastMonthRevenue > 0)
                percentDifference = (thisMonthRevenue - lastMonthRevenue) / lastMonthRevenue * 100;
            else
                percentDifference = 100;

            return new RevenueDTO
            {
                ThisMonthRevenue = thisMonthRevenue,
                LastMonthRevenue = lastMonthRevenue,
                PercentDifference = Math.Round(percentDifference, 2)
            };
        }

        public async Task<RevenueByYearDTO> GetRevenueByYear(int year)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = DateTime.Now;
            if (year != DateTime.Now.Year)
            { endDate = new DateTime(year, 12, 31); }

            RevenueByYearDTO dto = new RevenueByYearDTO();

            for(var month =  1; month <= endDate.Month; month++)
            {
                var startDateOfMonth = new DateTime(year, month, 1);
                var endDateOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month), 23, 59, 59);

                var revenue = await _orderRepository.MonthRevenue(startDateOfMonth, endDateOfMonth);

                dto.Labels.Add(startDateOfMonth.ToString("MMMM"));
                dto.Revenues.Add(revenue);
            }

            return dto;
        }

        public async Task<RevenueByDayDTO> GetRevenueByDay()
        {
            var startDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek); 
            var endDate = DateTime.Now; 

            var dto = new RevenueByDayDTO();

            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                var revenue = await _orderRepository.DayRevenue(date);
                var label = date.ToString("dddd");

                dto.Days.Add(label);
                dto.Revenues.Add(revenue);
            }

            return dto;
        }
    }
}
