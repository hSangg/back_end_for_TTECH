using AutoMapper;
using tech_project_back_end.Data;
using tech_project_back_end.Repository.IRepository;
using tech_project_back_end.Models;
using tech_project_back_end.DTO.Order;
using Microsoft.EntityFrameworkCore;
using tech_project_back_end.Controllers;

namespace tech_project_back_end.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _appDbContext;

        public OrderRepository(AppDbContext appDbContext)
        {
            this._appDbContext = appDbContext;
        }

        public async Task<List<Order>> GetList(DateTime startDate, DateTime endDate)
        {
            return await _appDbContext.Order
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .ToListAsync();
        }

        public async Task<long> MonthRevenue(DateTime startDateOfMonth, DateTime endDateOfMonth)
        {
            return await _appDbContext.Order
                 .Where(o => o.CreatedAt >= startDateOfMonth && o.CreatedAt <= endDateOfMonth)
                 .SumAsync(o => o.Total);
        }

        public async Task<long> DayRevenue(DateTime date)
        {
            return await _appDbContext.Order
                 .Where(o => o.CreatedAt.Date == date.Date)
                 .SumAsync(o => o.Total);
        }

        public async Task<Order> Create(Order order) {
            order.CreatedAt = DateTime.Now;
            await _appDbContext.Order.AddAsync(order);
            await _appDbContext.SaveChangesAsync();
            return order;
        }

        public async Task<Order> FindById(string id)
        {
            return await _appDbContext.Order.FirstOrDefaultAsync(p => p.OrderId == id);
        }

        public async Task<Order> UpdateState(string id,string state) {
            var existingOrder = await this.FindById(id);
            // Update the existing product
            if (existingOrder != null)
            {
                existingOrder.State = state;
                await _appDbContext.SaveChangesAsync();
            }
            return existingOrder;
        }

        public async Task<List<OrderDataTableDTO>> GetAll() {
            var listOrder = await _appDbContext.Order.Select(x => new OrderDataTableDTO
            {
                orderId = x.OrderId,
                customerName = x.Name,
                customerAddress = x.Address,
                customerEmail = x.Email,
                customerPhone = x.Phone,
                customerNote = x.note,
                deliveryFee = x.DeliveryFee,
                discountId = x.DiscountId,
                total = x.Total,
                createdAt = x.CreatedAt,

            }).ToListAsync();
            return listOrder;
        }

        public async Task<IEnumerable<dynamic>> GetAllWithDiscountOrderByDescending() {
            var orders = await _appDbContext.Order.Join(_appDbContext.User, o => o.UserId, u => u.UserId,
                (o, u) => new
                {
                    CustomerInfor = u,
                    OrderInfor = o,
                    DiscountInfor = _appDbContext.Discount.Where(d => d.DiscountId == o.DiscountId).FirstOrDefault()

                }
                ).OrderByDescending(x => x.OrderInfor.CreatedAt).ToListAsync();
            
            return orders;
        }

        public async Task<IEnumerable<dynamic>> GetALlByIdOrderByDescending(string id) {
            var orders = await _appDbContext.Order
                                .Where(o => o.OrderId.ToLower().Contains(id.ToLower()))
                                .Join(_appDbContext.User,
                                    o => o.UserId,
                                    u => u.UserId,
                                    (o, u) => new
                                    {
                                        CustomerInfor = u,
                                        OrderInfor = o
                                    })
                                .ToListAsync();

            var discounts = await _appDbContext.Discount.ToListAsync();

            var result = orders.AsEnumerable().Select(o => new
            {
                o.CustomerInfor,
                o.OrderInfor,
                DiscountInfor = discounts.FirstOrDefault(d => d.DiscountId == o.OrderInfor.DiscountId)
            })
            .OrderByDescending(x => x.OrderInfor.CreatedAt)
            .ToList();

            return result.Any() ? result : null;

        }

        public async Task<IEnumerable<dynamic>> GetByUserId(string userId) {
            var orders = await _appDbContext.Order
                .Join(
                    _appDbContext.DetailOrder,
                    o => o.OrderId,
                    od => od.OrderId,
                    (o, od) => new
                    {
                        OrderId = o.OrderId,
                        CreateOrderAt = o.CreatedAt,
                        UserId = o.UserId,
                        ProductId = od.ProductId,
                        QuantityPr = od.Quantity,
                        PricePr = od.Price,
                        Total = o.Total + o.DeliveryFee

                    })
                .Where(x => x.UserId == userId)
                .GroupBy(o => new
                {
                    o.OrderId,
                    o.CreateOrderAt,
                    o.UserId,
                    o.Total

                })
                .Select(g => new
                {
                    OrderInfo = new
                    {
                        g.Key.OrderId,
                        g.Key.CreateOrderAt,
                        g.Key.UserId,
                        g.Key.Total,

                    },
                    OrderDetails = g.Select(x => new
                    {
                        Product = _appDbContext.Product
                            .Where(p => p.ProductId == x.ProductId)
                            .Select(p => new
                            {
                                p.ProductId,
                                p.NamePr,
                                p.Detail,
                                p.Price,
                                Images = _appDbContext.Image
                                    .Where(i => i.ProductId == p.ProductId)
                                    .Select(i => i.ImageHref)
                                    .ToList()
                            })
                            .FirstOrDefault(),
                        QuantityPr = x.QuantityPr,
                        PricePr = x.PricePr
                    }).ToList()
                }).ToListAsync();
            return orders;
        }

        public async Task<int> Count() { return await _appDbContext.Order.CountAsync(); }
    }
}
