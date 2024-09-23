using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tech_project_back_end.Data;
using tech_project_back_end.Models;

namespace tech_project_back_end.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public CartController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }



        [HttpPost("GetUserTotalProduct")]
        public IActionResult GetUserTotalProduct([FromBody] string user_id)
        {

            var totalProduct = _appDbContext.Cart.Where(c => c.user_id == user_id).Select(x => x.product_id).Distinct().Count();
            return Ok(totalProduct);
        }

        [HttpPost("GetCartProduct")]
        public IActionResult GetCartProduct([FromBody] string user_id)
        {
            var productsInCart = from cart in _appDbContext.Set<Cart>()
                                 join product in _appDbContext.Set<Product>()
                                 on cart.product_id equals product.product_id
                                 join supplier in _appDbContext.Set<Supplier>()
                                 on product.supplier_id equals supplier.SupplierId
                                 where cart.user_id == user_id
                                 select new
                                 {
                                     Product = new
                                     {
                                         product_id = product.product_id,
                                         name_pr = product.name_pr,
                                         name_serial = product.name_serial,
                                         detail = product.detail,
                                         price = product.price,
                                         quantity_pr = product.quantity_pr,
                                         guarantee_period = product.guarantee_period,
                                         supplier_id = product.supplier_id,
                                     },
                                     quantity = cart.quantity,
                                     Category = _appDbContext.ProductCategory
                                        .Where(pc => pc.product_id == cart.product_id)
                                        .Join(_appDbContext.Category,
                                                pc => pc.category_id,
                                                c => c.category_id,
                                                    (pc, c) => new { c.category_id, c.category_name })
                                                    .SingleOrDefault(),
                                     Supplier = supplier,
                                     Image = _appDbContext.Set<Image>()
                                         .Where(i => i.ProductId == product.product_id)
                                         .FirstOrDefault()
                                 };

            return Ok(productsInCart);
        }
        [HttpPut("UpdateQuantity")]
        public IActionResult UpdateQuantity([FromBody] Cart cart)
        {
            var userId = cart.user_id; // replace this with your own method for getting the user ID
            var cartItem = _appDbContext.Set<Cart>()
                .FirstOrDefault(c => c.user_id == userId && c.product_id == cart.product_id);
            if (cartItem == null)
            {
                return NotFound($"Cart item with product ID {cart.product_id} not found");
            }

            if (cart.quantity == 0)
            {
                _appDbContext.Set<Cart>().Remove(cartItem);
            }
            else
            {
                cartItem.quantity = cart.quantity;
                _appDbContext.Set<Cart>().Update(cartItem);
            }

            _appDbContext.SaveChanges();
            return Ok();
        }

        [HttpDelete("EmptyCart")]
        public IActionResult EmptyCart(string user_id)
        {
            var cartItems = _appDbContext.Set<Cart>().Where(c => c.user_id == user_id);
            _appDbContext.Set<Cart>().RemoveRange(cartItems);
            _appDbContext.SaveChanges();
            return Ok();
        }


        [HttpPost("AddToCart")]
        public IActionResult AddToCart([FromBody] Cart cart)
        {

            var userId = cart.user_id; // replace this with your own method for getting the user ID
            var product = _appDbContext.Set<Product>().FirstOrDefault(p => p.product_id == cart.product_id);
            if (product == null)
            {
                return NotFound($"Product with ID {cart.product_id} not found");
            }

            var existingCartItem = _appDbContext.Set<Cart>()
                .FirstOrDefault(c => c.user_id == userId && c.product_id == cart.product_id);
            if (existingCartItem != null)
            {
                existingCartItem.quantity += cart.quantity;
                _appDbContext.Set<Cart>().Update(existingCartItem);
            }
            else
            {
                var cartItem = new Cart
                {
                    user_id = userId,
                    product_id = cart.product_id,
                    quantity = cart.quantity
                };
                _appDbContext.Set<Cart>().Add(cartItem);
            }

            _appDbContext.SaveChanges();
            return Ok(true);


        }







    }



}
