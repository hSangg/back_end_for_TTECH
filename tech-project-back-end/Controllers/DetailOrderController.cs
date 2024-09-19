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
            var result = _appDbContext.Detail_Order.Where(od => od.order_id == order_id).Select(x
                => new
                {
                    Product = _appDbContext.Product.Where(p => p.product_id == x.product_id).FirstOrDefault(),
                    Image = _appDbContext.Image.Where(i => i.product_id == x.product_id).FirstOrDefault(),
                    Quantity = x.quality,
                    Price = x.price,
                });

            return Ok(result);

        }

        [HttpPost("AddNewDetailOrder")]
        public IActionResult AddNewOrderDetail(List<Detail_Order> detailOrders)
        {
            foreach (var detailOrder in detailOrders)
            {
                _appDbContext.Detail_Order.Add(detailOrder);
                var product_id = detailOrder.product_id;
                var quantityDescrease = detailOrder.quality;



            }
            _appDbContext.SaveChanges();
            return Ok(detailOrders);
        }
    }
}
