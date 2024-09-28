using Microsoft.AspNetCore.Mvc;
using tech_project_back_end.Data;
using tech_project_back_end.DTO.Discount;
using tech_project_back_end.Models;
using tech_project_back_end.Services.IService;

namespace tech_project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountService _discountService;
        private readonly ILogger _logger;
        public DiscountController(IDiscountService discountService, ILogger logger)
        {
            _discountService = discountService;
            _logger = logger;
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetAllDiscount()
        {
            try
            {
                var voucherList = await _discountService.GetAllDiscountsAsync();
                return Ok(voucherList);
            } catch(Exception err)
            {
                _logger.LogError(err, err.Message);
                return StatusCode(500, err.Message);
            }
        }
        
        [HttpGet("GetCurrentDiscount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetDiscount(DateTime currentDate)
        {
            try
            {
                var voucherList = await _discountService.GetDiscountByCurrentDateAsync(currentDate);
                return Ok(voucherList);
            } catch(Exception err)
            {
                _logger.LogError(err, err.Message);
                return StatusCode(500, err.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddNewDiscount([FromBody] CreateDiscountDTO discount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newDiscount = await _discountService.CreateDiscountAsync(discount);
                return newDiscount != null
                    ? Ok(newDiscount)
                    : BadRequest("Failed to create supplier");
            } catch(Exception err)
            {
                _logger.LogError(err, err.Message);
                return StatusCode(500, err.Message);
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateDiscount(DiscountDTO discount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updateDiscount = await _discountService.UpdateDiscountAsync(discount);
                return updateDiscount != null
                    ? Ok(updateDiscount)
                    : NotFound($"Supplier with ID {discount.DiscountId} not found");
            } catch(Exception err)
            {
                _logger.LogError(err, err.Message);
                return StatusCode(500, err.Message);
            }
        }
        
        [HttpDelete]
        public async Task<ActionResult> DeleteDiscount(string discountId)
        {

            try
            {
                await _discountService.DeleteDiscountByIdAsync(discountId);
                return Ok("Successfully deleted discount");
            } catch(Exception err)
            {
                _logger.LogError(err, err.Message);
                return StatusCode(500, err.Message);
            }

        }
    }
}
