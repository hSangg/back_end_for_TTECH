using Microsoft.AspNetCore.Mvc;
using tech_project_back_end.Data;
using tech_project_back_end.Models;

namespace tech_project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetailOrderController : ControllerBase
    {

        private readonly AppDbContext _appDbContext;
        public DetailOrderController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet("GetOderDetailByOrderId")]
        public IActionResult GetOderDetailByOrderId(string order_id)
        {
            var result = _appDbContext.Detail_Order.Where(od => od.OrderId == order_id).Select(x
                => new
                {
                    Product = _appDbContext.Product.Where(p => p.product_id == x.ProductId).FirstOrDefault(),
                    Image = _appDbContext.Image.Where(i => i.product_id == x.ProductId).FirstOrDefault(),
                    Quantity = x.QuantityPr,
                    Price = x.PricePr,
                });

            return Ok(result);

        }

        [HttpPost("AddNewDetailOrder")]
        public IActionResult AddNewOrderDetail(List<Detail_Order> detailOrders)
        {
            foreach (var detailOrder in detailOrders)
            {
                _appDbContext.Detail_Order.Add(detailOrder);
                var product_id = detailOrder.ProductId;
                var quantityDescrease = detailOrder.QuantityPr;



            }
            _appDbContext.SaveChanges();
            return Ok(detailOrders);
        }
    }
}
