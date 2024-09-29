using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Mvc;
using tech_project_back_end.Data;
using tech_project_back_end.DTO.RevenueDTO;
using tech_project_back_end.Models;
using tech_project_back_end.Services;
using tech_project_back_end.Services.IService;

namespace tech_project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IRevenueService _revenueService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly ILogger _logger;
        public AdminController(AppDbContext appDbContext, ILogger logger, IRevenueService revenueService, IProductService productService, IOrderService orderService, IUserService userService)
        {
            _appDbContext = appDbContext;
            _revenueService = revenueService;
            _logger = logger;
            _productService = productService;
            _orderService = orderService;
            _userService = userService;
        }

        [HttpGet("GetRevenue")]
        public async Task<IActionResult> GetRevenue()
        {
            try
            {
                var revenueDto = await _revenueService.GetRevenue();
                return Ok(revenueDto);
            }
            catch (ArgumentException ex) 
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting revenue");
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }

        [HttpGet("GetTopSellerProduct")]
        public async Task<IActionResult> GetTopSellerProduct(int count)
        {
            try
            {
                var result = await _productService.GetTopSellerProducts(count);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting top seller product(s)");
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }

        [HttpGet("GetRevenueByYear")]
        public async Task<IActionResult> GetRevenueByYear(int year)
        {
            try
            {
                var result = await _revenueService.GetRevenueByYear(year);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Please recheck your argument");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching revenue by year");
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }

        // GET api/revenue/day
        [HttpGet("GetRevenueByDay")]
        public async Task<IActionResult> GetRevenueByDay()
        {
            try
            {
                var revenueDto = await _revenueService.GetRevenueByDay();
                return Ok(new
                {
                    day = revenueDto.Days,
                    revenue = revenueDto.Revenues
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Please recheck your argument");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching revenue by day in system");
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }

        [HttpGet("GetTotalCustomer")]
        public async Task<IActionResult> GetTotalCustomer()
        {
            try
            {
                var result = await _userService.GetTotalUser();
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Please recheck your argument");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching total user in system");
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }

        [HttpGet("GetTotalOrder")]
        public async Task<IActionResult> GetTotalOrder()
        {
            try
            {
                var result = await _orderService.GetTotalOrder();
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Please recheck your argument");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching total order in system");
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }
    }
}

