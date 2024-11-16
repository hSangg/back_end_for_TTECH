using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tech_project_back_end.DTO.Cart;
using tech_project_back_end.Services.IService;

namespace tech_project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger _logger;

        public CartController(ICartService cartService, ILogger logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        [Authorize]
        [HttpPost("GetUserTotalProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                var totalProduct = await _cartService.GetUserTotalProduct(userId);
                return Ok(totalProduct);
            } 
            catch(Exception err)
            {
                _logger.LogError(err, err.Message);
                return StatusCode(500, err.Message);
            }
        }

        [Authorize]
        [HttpPost("GetCartProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetCartProduct()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                var newCart = await _cartService.GetCartProduct(userId);
                return Ok(newCart);
            } 
            catch(Exception err)
            {
                _logger.LogError(err, err.Message);
                return StatusCode(500, err.Message);
            }
        }

        [Authorize]
        [HttpPost("AddToCart")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Add([FromBody] ModifyCartDTO entity)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                var newCart = await _cartService.AddToCart(entity, userId);
                return Ok(newCart);
            } 
            catch(Exception err)
            {
                _logger.LogError(err, err.Message);
                return StatusCode(500, err.Message);
            }
        }

        [Authorize]
        [HttpPut("UpdateQuantity")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Update([FromBody] ModifyCartDTO entity)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                var updateCart = await _cartService.UpdateQuantity(entity, userId);
                return Ok(updateCart);
            } 
            catch(Exception err)
            {
                _logger.LogError(err, err.Message);
                return StatusCode(500, err.Message);
            }
        }

        [Authorize]
        [HttpDelete("EmptyCart")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteByUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                await _cartService.EmptyCart(userId);
                return Ok("Successfully deleted cart");
            } 
            catch(Exception err)
            {
                _logger.LogError(err, err.Message);
                return StatusCode(500, err.Message);
            }
        }
    }
}