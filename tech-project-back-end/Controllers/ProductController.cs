using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tech_project_back_end.Data;
using tech_project_back_end.Models;

namespace tech_project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment eviroment;
        public ProductController(AppDbContext appDbContext, IWebHostEnvironment eviroment)
        {
            this._appDbContext = appDbContext;
            this.eviroment = eviroment;
        }

        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct(Product product)
        {
            try
            {
                _appDbContext.Product.Add(product);


                await _appDbContext.SaveChangesAsync();


                return Ok(new { product });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Failed to add product.");
            }
        }

        [HttpGet("getProductById")]
        public IActionResult GetProductById(string id)
        {
            try
            {
                var product = _appDbContext.Product
                    .Join(_appDbContext.Supplier,
                        p => p.supplier_id,
                        s => s.supplier_id,
                        (p, s) => new
                        {
                            Product = new
                            {
                                product_id = p.product_id,
                                name_pr = p.name_pr,
                                name_serial = p.name_serial,
                                detail = p.detail,
                                price = p.price,
                                quantity_pr = p.quantity_pr,
                                guarantee_period = p.guarantee_period
                            },
                            Category = _appDbContext.ProductCategory
                                        .Where(pc => pc.product_id == p.product_id)
                                        .Join(_appDbContext.Category,
                                                pc => pc.category_id,
                                                c => c.category_id,
                                                    (pc, c) => new { c.category_id, c.category_name })
                                                    .ToList(),
                            Supplier = new { s.supplier_id, s.supplier_name },
                            Image = _appDbContext.Image
                                .Where(i => i.product_id == p.product_id)
                                .FirstOrDefault()
                        })
                    .FirstOrDefault(p => p.Product.product_id == id);

                if (product != null)
                {
                    return Ok(product);
                }
                else
                {
                    return NotFound("Product not found");
                }
            }
            catch
            {
                return NotFound("Product not found");
            }
        }

        [HttpPost("GetProductBySearchQuery")]
        public IActionResult GetProductBySearchQuery([FromBody] string searchQuery)
        {
            var searchKeywords = searchQuery.ToLower().Split(' ');

            var products = _appDbContext.Product
    .Join(_appDbContext.Supplier,
        p => p.supplier_id,
        s => s.supplier_id,
        (p, s) => new
        {
            Product = new
            {
                product_id = p.product_id,
                name_pr = p.name_pr,
                name_serial = p.name_serial,
                detail = p.detail,
                price = p.price,
                quantity_pr = p.quantity_pr,
                guarantee_period = p.guarantee_period
            },
            Category = _appDbContext.ProductCategory
                .Where(pc => pc.product_id == p.product_id)
                .Join(_appDbContext.Category,
                    pc => pc.category_id,
                    c => c.category_id,
                    (pc, c) => new { c.category_id, c.category_name })
                .ToList(),
            Supplier = new { s.supplier_id, s.supplier_name },
            Image = _appDbContext.Image
                .Where(i => i.product_id == p.product_id)
                .FirstOrDefault()
        })
    .AsEnumerable() // Perform client-side evaluation from this point
    .Where(x =>
        searchKeywords.Any(keyword =>
            x.Product.name_pr.ToLower().Contains(keyword.ToLower()) ||
            x.Product.name_serial.ToLower().Contains(keyword.ToLower()) ||
            x.Product.detail.ToLower().Contains(keyword.ToLower()) || x.Product.product_id.ToLower().Contains(keyword.ToLower()) ||
            x.Category.Any(x => x.category_name.ToLower().Contains(keyword.ToLower())) == true
        )
    )
    .Distinct().Take(6).ToList();

            return Ok(products);
        }


        [HttpPost("GetProduct")]
        public IActionResult GetProduct([FromBody] Filter filter)
        {
            var productList = _appDbContext.Product
        .Join(_appDbContext.Supplier,
            p => p.supplier_id,
            s => s.supplier_id,
            (p, s) => new
            {
                Product = new
                {
                    product_id = p.product_id,
                    name_pr = p.name_pr,
                    name_serial = p.name_serial,
                    detail = p.detail,
                    price = p.price,
                    quantity_pr = p.quantity_pr,
                    guarantee_period = p.guarantee_period,

                },
                Category = _appDbContext.ProductCategory
                        .Where(pc => pc.product_id == p.product_id)
                        .Join(_appDbContext.Category,
                            pc => pc.category_id,
                            c => c.category_id,
                            (pc, c) => new { c.category_id, c.category_name })
                        .ToList(),

                Supplier = new { s.supplier_id, s.supplier_name },
                Image = _appDbContext.Image
                    .Where(i => i.product_id == p.product_id)
                    .FirstOrDefault()
            });


            // Filter by minimum and maximum price
            if (filter.MinPrice.HasValue)
            {
                productList = productList.Where(p => p.Product.price >= (ulong)filter.MinPrice.Value);
            }

            if (!string.IsNullOrEmpty(filter.SearchKey))
            {
                productList = productList
                    .Where(x =>
                        x.Product.name_pr.ToLower().Contains(filter.SearchKey.ToLower()) ||
                        x.Product.name_serial.ToLower().Contains(filter.SearchKey.ToLower()) ||
                        x.Product.detail.ToLower().Contains(filter.SearchKey.ToLower()) || x.Product.product_id.ToLower().Contains(filter.SearchKey.ToLower()) ||
                        x.Category.Any(c => c.category_name.ToLower().Contains(filter.SearchKey.ToLower())) == true

                );
            }

            if (filter.MaxPrice.HasValue)
            {
                productList = productList.Where(p => p.Product.price <= (ulong)filter.MaxPrice.Value);
            }

            // Filter by supplier name
            if (!string.IsNullOrEmpty(filter.SupplierId))
            {
                productList = productList.Where(p => p.Supplier.supplier_id == filter.SupplierId);
            }

            // Filter by category
            if (!string.IsNullOrEmpty(filter.CategoryId))
            {
                productList = productList
                    .Join(_appDbContext.ProductCategory,
                        p => p.Product.product_id,
                        pc => pc.product_id,
                        (p, pc) => new { p, pc })
                    .Where(x => x.pc.category_id == filter.CategoryId)
                    .Select(x => x.p);
            }

            // Sort by name or price
            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                switch (filter.SortBy.ToLower())
                {
                    case "name":
                        productList = (bool)filter.IsDescending ?
                            productList.OrderByDescending(p => p.Product.name_pr) :
                            productList.OrderBy(p => p.Product.name_pr);
                        break;
                    case "price":
                        productList = (bool)filter.IsDescending ?
                            productList.OrderByDescending(p => p.Product.price) :
                            productList.OrderBy(p => p.Product.price);
                        break;
                    default:
                        break;
                }
            }



            // Pagination
            int pageNumber = filter.PageNumber ?? 1;
            int pageSize = filter.PageSize ?? 10;

            var pagedProductList = productList.Skip((pageNumber - 1) * pageSize)
                                              .Take(pageSize)
                                              .ToList();

            var totalProductCount = productList.Count();
            var totalPages = (int)Math.Ceiling((double)totalProductCount / pageSize);

            var response = new
            {
                Products = pagedProductList,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalProducts = totalProductCount
            };

            return Ok(response);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProductById(string product_id)
        {
            try
            {
                var result = await DeleteImageFolder(product_id);
                var productsToDelete = _appDbContext.Product.Where(p => p.product_id == product_id);
                _appDbContext.Product.RemoveRange(productsToDelete);
                var productsImageToDelete = _appDbContext.Image.Where(i => i.product_id == product_id);
                _appDbContext.Image.RemoveRange(productsImageToDelete);
                _appDbContext.SaveChanges();

                return Ok("Add oke");


            }
            catch (Exception ex)
            {
                return BadRequest("Failed to upload image.");
            }

        }

        [HttpPost("AddMoreImageForProduct")]
        public async Task<IActionResult> AddMoreImageForProduct(IFormFileCollection formFileCollection, string product_id)
        {
            try
            {
                foreach (var formFile in formFileCollection)
                {

                    string imageUrl = await AddImage(formFile, product_id);


                }
                return Ok("Add oke");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Failed to upload image.");
            }
        }

        [HttpGet("GetAllImageOfProduct")]
        public IActionResult GetAllImageOfProduct(string product_id)
        {

            var result = _appDbContext.Image.Where(i => i.product_id == product_id);
            return Ok(result);

        }

        [HttpPost("UpaloadImage")]
        public async Task<string> UpaloadImage(IFormFileCollection formFileCollection)
        {
            foreach (var formFile in formFileCollection)
            {
                string productId = Path.GetFileNameWithoutExtension(formFile.FileName);
                string filePath = GetFilePath(productId);

                if (!System.IO.Directory.Exists(filePath))
                {
                    System.IO.Directory.CreateDirectory(filePath);
                }

                string fileExtension = Path.GetExtension(formFile.FileName);
                string imageName = $"{productId}{DateTimeOffset.Now:yyyyMMddHHmmssffff}{fileExtension}";
                string imagePath = Path.Combine(filePath, imageName);

                using (FileStream stream = System.IO.File.Create(imagePath))
                {
                    await formFile.CopyToAsync(stream);
                }

                string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";
                string relativePath = $"/Upload/product/{productId}/{imageName}";
                string imageUrl = baseUrl + relativePath;

                _appDbContext.Image.Add(new Image
                {
                    image_id = Guid.NewGuid().ToString()[..36],
                    product_id = productId,
                    image_href = imageUrl
                });

                await _appDbContext.SaveChangesAsync();
            }
            return "oke";
        }

        [HttpDelete("DeleteImageOfProduct")]
        public async Task<IActionResult> DeleteImageOfProduct(string product_id, string file_name)
        {
            var imageExit = _appDbContext.Image.Where(i => i.product_id == product_id && i.file_name == file_name);
            _appDbContext.RemoveRange(imageExit);
            _appDbContext.SaveChanges();
            DeleteImage(product_id, file_name);

            return Ok("");

        }

        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct([FromBody] Product updatedProduct)
        {
            if (updatedProduct == null)
            {
                return BadRequest("Invalid request data");
            }



            try
            {
                // Check if the specified product exists
                var existingProduct = await _appDbContext.Product
                    .FirstOrDefaultAsync(p => p.product_id == updatedProduct.product_id);

                if (existingProduct == null)
                {
                    return NotFound("Product not found");
                }

                decimal temp;
                if (!decimal.TryParse(updatedProduct.guarantee_period.ToString(), out temp) ||
                    !decimal.TryParse(updatedProduct.quantity_pr.ToString(), out temp) ||
                    !decimal.TryParse(updatedProduct.price.ToString(), out temp))
                {
                    return BadRequest(false);
                }

                // Update the existing product
                existingProduct.name_pr = updatedProduct.name_pr;
                existingProduct.name_serial = updatedProduct.name_serial;
                existingProduct.detail = updatedProduct.detail;
                existingProduct.price = updatedProduct.price;
                existingProduct.quantity_pr = updatedProduct.quantity_pr;
                existingProduct.guarantee_period = updatedProduct.guarantee_period;
                existingProduct.supplier_id = updatedProduct.supplier_id;

                await _appDbContext.SaveChangesAsync();

                return Ok("Product updated successfully");
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(409, "Concurrency conflict");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [NonAction]
        public async Task<string> AddImage(IFormFile formFile, string product_id)
        {
            string FilePath = GetFilePath(product_id);

            if (!System.IO.Directory.Exists(FilePath))
            {
                System.IO.Directory.CreateDirectory(FilePath);
            }

            string fileExtension = Path.GetExtension(formFile.FileName);
            string fileName = $"{DateTime.Now.Ticks}{fileExtension}";
            string ImagePath = Path.Combine(FilePath, fileName);

            using (FileStream stream = System.IO.File.Create(ImagePath))
            {
                await formFile.CopyToAsync(stream);
            }

            string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";
            string relativePath = $"/Upload/product/{product_id}/{fileName}";
            string imageUrl = baseUrl + relativePath;

            _appDbContext.Image.Add(new Image
            {
                image_id = Guid.NewGuid().ToString()[..36],
                product_id = product_id,
                image_href = imageUrl,
                file_name = fileName,

            });

            await _appDbContext.SaveChangesAsync();

            return imageUrl;
        }


        [NonAction]
        private string GetFilePath(string product_id)
        {
            return this.eviroment.WebRootPath + "\\Upload\\product\\" + product_id;
        }

        [NonAction]
        public async Task<string> DeleteImageFolder(string product_id)
        {
            string filePath = GetFilePath(product_id);

            if (System.IO.Directory.Exists(filePath))
            {
                System.IO.Directory.Delete(filePath, true);
            }

            return "oke";
        }

        [NonAction]
        public void DeleteImage(string product_id, string file_name)
        {
            string filePath = GetFilePath(product_id);
            string imagePath = Path.Combine(filePath, file_name);

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
        }

        [HttpDelete("RemoveImageFolder")]
        public async Task<IActionResult> RemoveImageFolder(string product_id)
        {
            string filePath = GetFilePath(product_id);

            if (System.IO.Directory.Exists(filePath))
            {
                System.IO.Directory.Delete(filePath, true);
            }

            return Ok("oke");
        }

    }
}
