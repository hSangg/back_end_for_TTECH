using Microsoft.AspNetCore.Mvc;
using tech_project_back_end.DTO.DetailOrder;
using tech_project_back_end.Services.IService;

namespace tech_project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetailOrderController : ControllerBase
    {

        private readonly IDetailOrderService _detailOrderService;
        private readonly ILogger<DetailOrderController> _logger;

        public DetailOrderController(IDetailOrderService detailOrderService, ILogger<DetailOrderController> logger)
        {
            this._detailOrderService = detailOrderService;
            this._logger = logger;
        }

        [HttpGet("GetOderDetailByOrderId")]
        public async Task<IActionResult> GetOderDetailByOrderId(string id)
        {
            if (id == null)
            {
                return BadRequest("Invalid request data");
            }
            try
            {
                var result = await _detailOrderService.GetOderDetailByOrderId(id);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting order detail by order id");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("AddNewDetailOrder")]
        public async Task<IActionResult> AddNewOrderDetail(List<DetailOrderDTO> detailOrderDTOs)
        {
            if (detailOrderDTOs == null)
            {
                return BadRequest("Invalid request data");
            }
            try
            {
                var result = await _detailOrderService.AddNewDetailOrder(detailOrderDTOs);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding new order detail");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
            
        }
    }
}
