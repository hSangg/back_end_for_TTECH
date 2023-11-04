using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tech_project_back_end.Data;
using tech_project_back_end.Models;

namespace tech_project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public CartController (AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

       

        [HttpPost("GetUserTotalProduct")]
        public IActionResult GetUserTotalProduct([FromBody] string user_id) {
        
            var totalProduct = _appDbContext.Cart.Where(c => c.user_id == user_id).Count();
            return Ok(totalProduct);
        }

        [HttpPost("GetCartProduct")]
        public IActionResult GetCartProduct([FromBody] string user_id)
        {
            var productsInCart = from cart in _appDbContext.Set<Cart>()
                                 join product in _appDbContext.Set<Product>()
                                 on cart.product_id equals product.product_id
                                 join supplier in _appDbContext.Set<Supplier>()
                                 on product.supplier_id equals supplier.supplier_id
                                 where cart.user_id == user_id
                                 select new 
                                 {
                                     Product = new {
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
                                     Category = _appDbContext.Product_Category
                                        .Where(pc => pc.ProductId == cart.product_id)
                                        .Join(_appDbContext.Category,
                                                pc => pc.CategoryId,
                                                c => c.category_id,
                                                    (pc, c) => new { c.category_id, c.category_name })
                                                    .SingleOrDefault(),
                                     Supplier = supplier,
                                     Image = _appDbContext.Set<Image>()
                                         .Where(i => i.product_id == product.product_id)
                                         .FirstOrDefault()
                                 };

            return Ok(productsInCart);
        }

    }


    
}
