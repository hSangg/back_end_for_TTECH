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
            var result = _appDbContext.DetailOrder.Where(od => od.OrderId == order_id).Select(x
                => new
                {
                    Product = _appDbContext.Product.Where(p => p.ProductId == x.ProductId).FirstOrDefault(),
                    Image = _appDbContext.Image.Where(i => i.ProductId == x.ProductId).FirstOrDefault(),
                    Quantity = x.Quantity,
                    Price = x.Price,
                });

            return Ok(result);

        }

        [HttpPost("AddNewDetailOrder")]
        public IActionResult AddNewOrderDetail(List<DetailOrder> detailOrders)
        {
            foreach (var detailOrder in detailOrders)
            {
                _appDbContext.DetailOrder.Add(detailOrder);
                var product_id = detailOrder.ProductId;
                var quantityDescrease = detailOrder.Quantity;



            }
            _appDbContext.SaveChanges();
            return Ok(detailOrders);
        }
    }
}
