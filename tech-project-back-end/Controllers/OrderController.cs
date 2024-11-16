using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using tech_project_back_end.DTO.Order;
using tech_project_back_end.Services.IService;

namespace tech_project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            this._orderService = orderService;
            this._logger = logger;
        }

        [Authorize]
        [HttpPost("AddNewOrder")]
        public async Task<IActionResult> AddNewOrder(OrderDTO orderDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _orderService.CreateOrder(orderDTO);
                return result != null
                    ? Ok(result)
                    : StatusCode(422, "Failed to create order due to invalid business logic."); ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating order");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Policy = "ADMIN")]
        [HttpPut("UpdateStateOrder")]
        public async Task<IActionResult> UpdateStateOrder(string orderId, string state)
        {
            if (orderId == null || state == null)
            {
                return BadRequest("Invalid request data");
            }

            try
            {
                var result = await _orderService.UpdateStateOrder(orderId, state);

                return Ok("order updated successfully");
            }
            catch(KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Order not found");
                return NotFound(ex.Message);
            }
            catch (DbUpdateConcurrencyException ex1)
            {
                _logger.LogError(ex1, "Concurrency conflict");
                return StatusCode(409, "Concurrency conflict");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Internal server error: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Policy = "ADMIN")]
        [HttpGet("GetExcelFileData")]
        public async Task<IActionResult> GetExcelFileData()
        {
            try
            {   
                var fileData = await _orderService.GetExcelFileData();

                Response.Headers.Add("Content-Disposition", "attachment; filename=OrderList.xlsx");

                return File(fileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Policy = "ADMIN")]
        [HttpGet("GetAllOrder")]
        public async Task<IActionResult> GetAllOrder()
        {
            try
            {   
                var orders = await _orderService.GetAllOrder();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all orders");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetOrderById")]
        public async Task<IActionResult> GetOrderById(string order_id)
        {

            if (order_id == null)
            {
                return BadRequest("Invalid request data");
            }
            try
            {
                var orders = await _orderService.GetById(order_id);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting order by id");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        [Authorize]
        [HttpPost("GetOrderByUserId")]
        public async Task<IActionResult> GetOrderByUserId([FromBody] string userId)
        {
            var userIdToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest("Invalid request data");
            }
            if(userIdToken != userId)
            {
                return Forbid();
            }
            try
            {
                var orders = await _orderService.GetByUserId(userId);

                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting order by user id");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
