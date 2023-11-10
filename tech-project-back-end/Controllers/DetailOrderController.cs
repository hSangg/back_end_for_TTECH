using Microsoft.AspNetCore.Http;
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

        [HttpPost("AddNewDetailOrder")]
        public IActionResult AddNewOrderDetail(List<Detail_Order> detailOrders)
        {
            foreach (var detailOrder in detailOrders)
            {
                _appDbContext.Detail_Order.Add(detailOrder);
            }
            _appDbContext.SaveChanges();
            return Ok(detailOrders);
        }
    }
}
