using Microsoft.AspNetCore.Mvc;
using tech_project_back_end.Data;

namespace tech_project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public DiscountController (AppDbContext appDbContext)
        {
            this._appDbContext = appDbContext;
        }


        [HttpGet]
        public IActionResult GetAllDiscount()
        {
            var voucherList = _appDbContext.Discount.ToList();
            return Ok(voucherList);
        }
    }
}
