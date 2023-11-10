using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using tech_project_back_end.Data;
using tech_project_back_end.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        public async Task<IActionResult> AddProduct(IFormFile formFile, int quantity_pr, string name_serial, string detail, string product_name, string supplier_id, ulong price, int guarantee_period)
        {
            try
            {
                var product_id = Guid.NewGuid().ToString()[..36];


                var product = new Product
                {
                    product_id = product_id,
                    name_pr = product_name,
                    supplier_id = supplier_id,
                    price = price,
                    guarantee_period = guarantee_period,
                    detail = detail,
                    quantity_pr = quantity_pr,
                    name_serial = name_serial,
                };


                _appDbContext.Product.Add(product);


                await _appDbContext.SaveChangesAsync();

                string imageUrl = await AddImage(formFile, product_id);


                return Ok(new { product, imageUrl });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Failed to upload image.");
            }
        }

        [HttpPost("getProductById")]
        public IActionResult GetProductById([FromBody] string id)
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
                            Category = _appDbContext.Product_Category
                                        .Where(pc => pc.ProductId == p.product_id)
                                        .Join(_appDbContext.Category,
                                                pc => pc.CategoryId,
                                                c => c.category_id,
                                                    (pc, c) => new { c.category_id, c.category_name })
                                                    .SingleOrDefault(),
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
            Category = _appDbContext.Product_Category
                .Where(pc => pc.ProductId == p.product_id)
                .Join(_appDbContext.Category,
                    pc => pc.CategoryId,
                    c => c.category_id,
                    (pc, c) => new { c.category_id, c.category_name })
                .SingleOrDefault(),
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
            x.Product.detail.ToLower().Contains(keyword.ToLower()) ||
            x.Category?.category_name.ToLower().Contains(keyword.ToLower()) == true
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
                Category = _appDbContext.Product_Category
                        .Where(pc => pc.ProductId == p.product_id)
                        .Join(_appDbContext.Category,
                            pc => pc.CategoryId,
                            c => c.category_id,
                            (pc, c) => new { c.category_id, c.category_name })
                        .SingleOrDefault(),

                Supplier = new { s.supplier_id, s.supplier_name },
                Image = _appDbContext.Image
                    .Where(i => i.product_id == p.product_id)
                    .FirstOrDefault()
            });




            // Filter by minimum and maximum price
            if (filter.MinPrice.HasValue)
            {
                productList = productList.Where(p => p.Product.price >=  (ulong)filter.MinPrice.Value);
            }

            if (!string.IsNullOrEmpty(filter.SearchKey))
            {
                productList = productList
                    .Where(x =>
                        x.Product.name_pr.ToLower().Contains(filter.SearchKey.ToLower()) ||
                        x.Product.name_serial.ToLower().Contains(filter.SearchKey.ToLower()) ||
                        x.Product.detail.ToLower().Contains(filter.SearchKey.ToLower()) ||
                        x.Category.category_name.ToLower().Contains(filter.SearchKey.ToLower()) == true
                    
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
                    .Join(_appDbContext.Product_Category,
                        p => p.Product.product_id,
                        pc => pc.ProductId,
                        (p, pc) => new { p, pc })
                    .Where(x => x.pc.CategoryId == filter.CategoryId)
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






        [HttpPut("AddMoreImageForProduct")]
        public async Task<IActionResult> AddMoreImageForProduct(IFormFile formFile, string product_id)
        {
            try
            {
                string imageUrl = await AddImage(formFile, product_id);

                return Ok(imageUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Failed to upload image.");
            }
        }

        [HttpPost("GetAllImageOfProduct")]
        public async Task<IActionResult> GetAllImageOfProduct([FromBody] string product_id)
        {
            List<string> Imageurl = new List<string>();
            string hosturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            try
            {
                string Filepath = GetFilePath(product_id);

                if (System.IO.Directory.Exists(Filepath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(Filepath);
                    FileInfo[] fileInfos = directoryInfo.GetFiles();
                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        string filename = fileInfo.Name;
                        string imagepath = Filepath + "\\" + filename;
                        if (System.IO.File.Exists(imagepath))
                        {
                            string _Imageurl = hosturl + "/Upload/product/" + product_id + "/" + filename;
                            Imageurl.Add(_Imageurl);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
            }
            return Ok(Imageurl);
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



        [NonAction]
        public async Task<string> AddImage(IFormFile formFile, string product_id)
        {
            string FilePath = GetFilePath(product_id);

            if (!System.IO.Directory.Exists(FilePath))
            {
                System.IO.Directory.CreateDirectory(FilePath);
            }

            string fileExtension = Path.GetExtension(formFile.FileName);
            string fileName = $"{product_id}_{DateTime.Now.Ticks}{fileExtension}";
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
                image_href = imageUrl
            });

            await _appDbContext.SaveChangesAsync();

            return imageUrl;
        }


        [NonAction]
        private string GetFilePath(string product_id)
        {
            return this.eviroment.WebRootPath + "\\Upload\\product\\" + product_id;
        }

    }
}
