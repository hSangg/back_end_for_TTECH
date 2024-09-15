using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using tech_project_back_end.Data;
using tech_project_back_end.Models;
using tech_project_back_end.Services;

namespace tech_project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IEmailService _emailService;
        public OrderController(AppDbContext appDbContext, IEmailService emailService)
        {
            _appDbContext = appDbContext;
            _emailService = emailService;
        }

        [HttpPost("AddNewOrder")]
        public IActionResult AddNewOrder(Order order)
        {
            _appDbContext.Order.Add(order);
            order.CreateOrderAt = DateTime.Now;
            _appDbContext.SaveChanges();
            return Ok(order);


        }



        [HttpPut("UpdateStateOrder")]
        public async Task<IActionResult> UpdateStateOrder(string orderId, string state)
        {
            if (orderId == null || state == null)
            {
                return BadRequest("Invalid request data");
            }

            try
            {
                // Check if the specified product exists
                var existingOrder = await _appDbContext.Order
                    .FirstOrDefaultAsync(p => p.OrderId == orderId);

                if (existingOrder == null)
                {
                    return NotFound("Product not found");
                }

                // Update the existing product
                existingOrder.State = state;

                await _appDbContext.SaveChangesAsync();

                return Ok("order updated successfully");
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(409, "Concurrency conflict");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetExcelFileData")]
        public IActionResult GetExcelFileData()
        {
            var orderList = GetOrderData();

            using (XLWorkbook wb = new())
            {
                wb.AddWorksheet(orderList, "Order Record");
                using (MemoryStream ms = new())
                {
                    wb.SaveAs(ms);

                    // Set the Content-Disposition header to trigger file download
                    Response.Headers.Add("Content-Disposition", "attachment; filename=OrderList.xlsx"); // Change the file extension to xlsx

                    return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                }
            }
        }
        [NonAction]
        private DataTable GetOrderData()
        {
            DataTable dt = new DataTable();
            dt.TableName = "OrderTable";
            dt.Columns.Add("orderId", typeof(string));
            dt.Columns.Add("customerName", typeof(string));
            dt.Columns.Add("customerEmail", typeof(string));
            dt.Columns.Add("customerPhone", typeof(string));

            dt.Columns.Add("customerNote", typeof(string));
            dt.Columns.Add("deliveryFee", typeof(int));
            dt.Columns.Add("discountId", typeof(string));
            dt.Columns.Add("total", typeof(long));
            dt.Columns.Add("createdAt", typeof(DateTime));

            var orderList = _appDbContext.Order.Select(x => new
            {
                orderId = x.OrderId,
                customerName = x.Name,
                customerAddress = x.Address,
                customerEmail = x.Email,
                customerPhone = x.Phone,
                customerNote = x.Note,

                deliveryFee = x.DeliveryFee,
                discountId = x.Discount,
                total = x.Total,
                createdAt = x.CreateOrderAt,

            }).ToList();
            if (orderList.Count > 0)
            {
                orderList.ForEach(item =>
                {
                    dt.Rows.Add(item.orderId, item.customerName, item.customerEmail, item.customerPhone, item.customerNote, item.deliveryFee, item.discountId, item.total, item.createdAt);
                });
            }

            return dt;
        }

        [HttpGet("GetAllOrder")]
        public IActionResult GetAllOrder()
        {
            var orders = _appDbContext.Order.Join(_appDbContext.User, o => o.UserId, u => u.user_id,
                (o, u) => new
                {
                    CustomerInfor = u,
                    OrderInfor = o,
                    DiscountInfor = _appDbContext.Discount.Where(d => d.DiscountId == o.Discount).FirstOrDefault()

                }
                ).OrderByDescending(x => x.OrderInfor.CreateOrderAt);

            return Ok(orders);


        }

        [HttpGet("GetOrderById")]
        public IActionResult GetOrderById(string order_id)
        {
            var isExit = _appDbContext.Order.FirstOrDefault(o => o.OrderId == order_id);
            if (isExit != null) { return NotFound("Order not found"); }

            var orders = _appDbContext.Order.Where(o => o.OrderId.ToLower().Contains(order_id.ToLower())).Join(_appDbContext.User, o => o.UserId, u => u.user_id,
                (o, u) => new
                {
                    CustomerInfor = u,
                    OrderInfor = o,
                    DiscountInfor = _appDbContext.Discount.Where(d => d.DiscountId == o.Discount).FirstOrDefault()

                }
                ).OrderByDescending(x => x.OrderInfor.CreateOrderAt);

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
