using Microsoft.AspNetCore.Mvc;
using tech_project_back_end.Data;
using tech_project_back_end.Models;

namespace tech_project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public DiscountController(AppDbContext appDbContext)
        {
            this._appDbContext = appDbContext;
        }


        [HttpGet]
        public IActionResult GetAllDiscount()
        {
            var voucherList = _appDbContext.Discount.ToList();
            return Ok(voucherList);
        }

        [HttpPost]
        public IActionResult AddNewDiscount(Discount discount)
        {
            _appDbContext.Discount.Add(discount);
            _appDbContext.SaveChanges();

            return Ok(discount);
        }

        [HttpPut]
        public IActionResult UpdateDiscount(Discount discount)
        {
            var isExit = _appDbContext.Discount.FirstOrDefault(x => x.DiscountId == discount.DiscountId);
            if (isExit == null) { return BadRequest("discount_id not found"); }

            isExit = discount;
            _appDbContext.SaveChanges();

            return Ok(isExit);

        }

        [HttpGet("GetCurrentDiscount")]
        public IActionResult GetDiscount(DateTime currentDate)
        {
            var isExit = _appDbContext.Discount.FirstOrDefault(dis => dis.DiscountDateFrom < currentDate && dis.DiscountDateTo > currentDate);
            if (isExit == null) return BadRequest("This date not cotain a discount_id");
            return Ok(isExit);

        }

        [HttpDelete]
        public IActionResult DeleteDiscount(string discountId)
        {

            var isExit = _appDbContext.Discount.FirstOrDefault(x => x.DiscountId == discountId);
            if (isExit == null) { return BadRequest("discount_id not found"); }

            _appDbContext.Remove(isExit);
            _appDbContext.SaveChanges();

            return Ok("Discount deleted.");

        }
    }
}
