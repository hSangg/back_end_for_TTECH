using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tech_project_back_end.Data;
using tech_project_back_end.Models;

namespace tech_project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public OrderController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpPost("AddNewOrder")]
        public IActionResult AddNewOrder(Order order)
        {
            _appDbContext.Order.Add(order);
            order.CreateOrderAt = DateTime.Now;
            _appDbContext.SaveChanges();
            return Ok(order);


        }

        [HttpGet("GetAllOrder")]
        public IActionResult GetAllOrder()
        {
            var orders = _appDbContext.Order.Join(_appDbContext.User, o => o.UserId, u => u.user_id,
                (o,u) => new
                {
                    CustomerInfor = u,
                    OrderInfor = o,
                    DiscountInfor = _appDbContext.Discount.Where(d => d.DiscountId == o.Discount).FirstOrDefault()

                }
                );

            return Ok(orders);


        }


        [HttpPost("GetOrderByUserId")]
        public IActionResult GetOrderByUserId([FromBody] string userId)
        {
            var orders = _appDbContext.Order
                .Join(
                    _appDbContext.Detail_Order,
                    o => o.OrderId,
                    od => od.OrderId,
                    (o, od) => new
                    {
                        OrderId = o.OrderId,
                        CreateOrderAt = o.CreateOrderAt,
                        UserId = o.UserId,
                        ProductId = od.ProductId,
                        QuantityPr = od.QuantityPr,
                        PricePr = od.PricePr,
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
                            .Where(p => p.product_id == x.ProductId)
                            .Select(p => new
                            {
                                p.product_id,
                                p.name_pr,
                                p.detail,
                                p.price,
                                Images = _appDbContext.Image
                                    .Where(i => i.product_id == p.product_id)
                                    .Select(i => i.image_href)
                                    .ToList()
                            })
                            .FirstOrDefault(),
                        QuantityPr = x.QuantityPr,
                        PricePr = x.PricePr
                    }).ToList()
                })
                .ToList();

            return Ok(orders);
        }


    }
}
