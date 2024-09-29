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
        private readonly ILogger<OrderController> _logger;
        private readonly IMapper _mapper;

        public OrderRepository(AppDbContext appDbContext, ILogger<OrderController> logger, IMapper mapper)
        {
            this._appDbContext = appDbContext;
            this._logger = logger;
            this._mapper = mapper;
        }

        public async Task<Order> Create(Order order) {
            order.createdAt = DateTime.Now;
            await _appDbContext.Order.AddAsync(order);
            await _appDbContext.SaveChangesAsync();
            return order;
        }

        public async Task<Order> FindById(string id)
        {
            return await _appDbContext.Order.FirstOrDefaultAsync(p => p.order_id == id);
        }

        public async Task<Order> UpdateState(string id,string state) {
            var existingOrder = await this.FindById(id);
            // Update the existing product
            if (existingOrder != null)
            {
                existingOrder.state = state;
                await _appDbContext.SaveChangesAsync();
            }
            return existingOrder;
        }

        public async Task<List<OrderDataTableDTO>> GetAll() {
            var listOrder = await _appDbContext.Order.Select(x => new OrderDataTableDTO
            {
                orderId = x.order_id,
                customerName = x.name,
                customerAddress = x.address,
                customerEmail = x.email,
                customerPhone = x.phone,
                customerNote = x.note,
                deliveryFee = x.delivery_fee,
                discountId = x.discount_id,
                total = x.total,
                createdAt = x.createdAt,

            }).ToListAsync();
            return listOrder;
        }

        public async Task<IEnumerable<dynamic>> GetAllWithDiscountOrderByDescending() {
            var orders = await _appDbContext.Order.Join(_appDbContext.User, o => o.user_id, u => u.UserId,
                (o, u) => new
                {
                    CustomerInfor = u,
                    OrderInfor = o,
                    DiscountInfor = _appDbContext.Discount.Where(d => d.DiscountId == o.discount_id).FirstOrDefault()

                }
                ).OrderByDescending(x => x.OrderInfor.createdAt).ToListAsync();

            return orders;
        }

        public async Task<IEnumerable<dynamic>> GetALlByIdOrderByDescending(string id) {
            var isExit = this.FindById(id);

            if (isExit != null)
            {

                var orders = await _appDbContext.Order.Where(o => o.order_id.ToLower().Contains(id.ToLower())).Join(_appDbContext.User, o => o.user_id, u => u.UserId,
                    (o, u) => new
                    {
                        CustomerInfor = u,
                        OrderInfor = o,
                        DiscountInfor = _appDbContext.Discount.Where(d => d.DiscountId == o.discount_id).FirstOrDefault()

                    }
                    ).OrderByDescending(x => x.OrderInfor.createdAt).ToListAsync();
                return orders;
            }
            return null;
        }

        public async Task<IEnumerable<dynamic>> GetByUserId(string userId) {
            var orders = await _appDbContext.Order
                .Join(
                    _appDbContext.DetailOrder,
                    o => o.order_id,
                    od => od.order_id,
                    (o, od) => new
                    {
                        OrderId = o.order_id,
                        CreateOrderAt = o.createdAt,
                        UserId = o.user_id,
                        ProductId = od.product_id,
                        QuantityPr = od.quality,
                        PricePr = od.price,
                        Total = o.total + o.delivery_fee

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
                            .Where(p => p.product_id == x.ProductId)
                            .Select(p => new
                            {
                                p.product_id,
                                p.name_pr,
                                p.detail,
                                p.price,
                                Images = _appDbContext.Image
                                    .Where(i => i.ProductId == p.product_id)
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
    }
}
