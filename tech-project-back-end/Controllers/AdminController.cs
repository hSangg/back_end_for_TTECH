using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using tech_project_back_end.Data;
using tech_project_back_end.Models;

namespace tech_project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public AdminController(AppDbContext appDbContext) {
            _appDbContext = appDbContext;
        }

        [HttpGet("GetRevenue")]
        public IActionResult GetRevenue ()
        {
            DateTime currentMonthStart = DateTime.Now.AddDays(-1 * DateTime.Now.Day + 1).Date;
            DateTime currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1);
            DateTime lastMonthStart = currentMonthStart.AddMonths(-1).AddDays(-1 * DateTime.Now.Day + 1).Date;
            DateTime lastMonthEnd = lastMonthStart.AddMonths(1).AddDays(-1);

            List<Order> ordersThisMonth = _appDbContext.Order.Where(o => o.CreateOrderAt >= currentMonthStart && o.CreateOrderAt <= currentMonthEnd).ToList();
            List<Order> ordersLastMonth = _appDbContext.Order.Where(o => o.CreateOrderAt >= lastMonthStart && o.CreateOrderAt <= lastMonthEnd).ToList();

            decimal thisMonthRevenue = ordersThisMonth.Sum(o => o.Total - o.DeliveryFee);
            decimal lastMonthRevenue = ordersLastMonth.Sum(o => o.Total - o.DeliveryFee);
            decimal percentDifference;
            if (lastMonthRevenue > 0)
                percentDifference = (thisMonthRevenue - lastMonthRevenue) / lastMonthRevenue * 100;
            else  percentDifference = 100;

            return Ok(new { ThisMonthRevenue = thisMonthRevenue, LastMonthRevenue = lastMonthRevenue, PercentDifference = Math.Round(percentDifference, 2) });
        }

        [HttpGet("GetTopSellerProduct")]
        public IActionResult GetTopSellerProduct(int count) {
            var subquery = _appDbContext.Detail_Order
                            .GroupBy(dt => dt.ProductId)
                            .Select(g => new
                            {
                                ProductId = g.Key,
                                TotalQuantitySold = g.Sum(dt => dt.QuantityPr)
                            });

            var result = subquery
                            .Join(_appDbContext.Product,
                                sq => sq.ProductId,
                                p => p.product_id,
                                (sq, p) => new
                                {
                                    ProductId = sq.ProductId,
                                    TotalQuantitySold = sq.TotalQuantitySold,
                                    ProductName = p.name_pr,
                                    Image = _appDbContext.Image.FirstOrDefault(i => i.product_id == sq.ProductId)
                                })
                            .OrderByDescending(p => p.TotalQuantitySold)
                            .Take(count)
                            .ToList();

            return Ok(result);
        }
        [HttpGet("GetRevenueByYear")]
        public IActionResult GetRevenueByYear(int year)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = DateTime.Now; // Current date

            var labels = new List<string>();
            var revenues = new List<long>();

            for (var month = 1; month <= endDate.Month; month++)
            {
                var startDateOfMonth = new DateTime(year, month, 1);
                var endDateOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month), 23, 59, 59);

                var revenue = _appDbContext.Order
                    .Where(o => o.CreateOrderAt >= startDateOfMonth && o.CreateOrderAt <= endDateOfMonth)
                    .Sum(o => o.Total);

                labels.Add(startDateOfMonth.ToString("MMMM"));
                revenues.Add(revenue);
            }

            var result = new
            {
                labels,
                revenues
            };

            return Ok(result);
        }
        // GET api/revenue/day
        [HttpGet("GetRevenueByDay")]
        public IActionResult GetRevenueByDay()
        {
            var startDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek); // Start from Sunday
            var endDate = DateTime.Now; // Current date

            var revenueByDay = new Dictionary<string, long>();

            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                var revenue = _appDbContext.Order
                    .Where(o => o.CreateOrderAt.Date == date.Date)
                    .Sum(o => o.Total);

                var label = date.ToString("dddd");

                revenueByDay[label] = revenue;
            }

            var result = new
            {
                day = revenueByDay.Keys,
                revenue = revenueByDay.Values
            };

            return Ok(result);
        }

        [HttpGet("GetTotalCustomer")]
        public IActionResult GetTotalCustomer()
        {
            var total = _appDbContext.User.Count();

            return Ok(total);
        }

        [HttpGet("GetTotalOrder")]
        public IActionResult GetTotalOrder()
        {
            var total = _appDbContext.Order.Count();

            return Ok(total);
        }
    }
}

