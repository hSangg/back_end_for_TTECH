using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Runtime.CompilerServices;
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
        public async Task<IActionResult> AddProduct(IFormFile formFile, string product_id, int quantity_pr, string name_serial, string detail, string product_name, string supplier_id, int price, int guarantee_period)
        {
            try
            {
                string imageUrl = await AddImage(formFile, product_id);

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

                return Ok(new { product, imageUrl });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Failed to upload image.");
            }
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
                                              guarantee_period = p.guarantee_period
                                          },
                                          Supplier = new { s.supplier_id, s.supplier_name },
                                          Image = _appDbContext.Image
                                                      .Where(i => i.product_id == p.product_id)
                                                      .FirstOrDefault()
                                      });

            // Filter by minimum and maximum price
            if (filter.MinPrice.HasValue)
            {
                productList = productList.Where(p => p.Product.price >= filter.MinPrice.Value);
            }

            if (!string.IsNullOrEmpty(filter.SearchKey))
            {
                productList = productList.Where(p =>
                    p.Product.name_pr.ToLower().Contains(filter.SearchKey.ToLower()) ||
                    p.Product.name_serial.ToLower().Contains(filter.SearchKey.ToLower())
                );
            }

            if (filter.MaxPrice.HasValue)
            {
                productList = productList.Where(p => p.Product.price <= filter.MaxPrice.Value);
            }

           
            // Filter by supplier name
            if (!string.IsNullOrEmpty(filter.SupplierId))
            {
                productList = productList.Where(p => p.Supplier.supplier_id == filter.SupplierId);
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

        [HttpGet("GetMultiImage")]
        public async Task<IActionResult> GetMultiImage(string product_id)
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
