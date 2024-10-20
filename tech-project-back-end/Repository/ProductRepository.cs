using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tech_project_back_end.Data;
using tech_project_back_end.DTO;
using tech_project_back_end.Helpter;
using tech_project_back_end.Models;
using tech_project_back_end.Models.ViewModel;
using tech_project_back_end.Repository.IRepository;

namespace tech_project_back_end.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<ProductRepository> _logger;
        private readonly Cloudinary _cloudinary;

        public ProductRepository(AppDbContext appDbContext, ILogger<ProductRepository> logger, Cloudinary cloudinary)
        {
            _appDbContext = appDbContext;
            _logger = logger;
            _cloudinary = cloudinary;
        }

        public async Task<List<TopSellerProductDTO>> TopSeller(int count)
        {
            var subquery = _appDbContext.DetailOrder
                .GroupBy(dt => dt.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalQuantitySold = g.Sum(dt => dt.Quantity)
                });

            var result = await subquery
                .Join(_appDbContext.Product.Where(p => !p.IsDeleted),
                    sq => sq.ProductId,
                    p => p.ProductId,
                    (sq, p) => new TopSellerProductDTO
                    {
                        ProductId = sq.ProductId,
                        TotalQuantitySold = sq.TotalQuantitySold,
                        ProductName = p.NamePr,
                        Image = _appDbContext.Image
                            .Where(i => i.ProductId == sq.ProductId)
                            .Select(i => new ImageDTO
                            {
                                ImageId = i.ImageId,
                                ProductId = i.ProductId,
                                ImageHref = i.ImageHref
                            })
                            .ToList()
                    })
                .OrderByDescending(p => p.TotalQuantitySold)
                .Take(count)
                .ToListAsync();

            return result;
        }

        public async Task AddProductAsync(Product product)
        {
            _appDbContext.Product.Add(product);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<ProductDTO> GetProductByIdAsync(string id)
        {
            var product = await _appDbContext.Product
                .Include(p => p.Supplier)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.ProductId == id && !p.IsDeleted);

            if (product == null) return null;

            return new DTO.ProductDTO
            {
                ProductId = product.ProductId,
                NamePr = product.NamePr,
                NameSerial = product.NameSerial,
                Detail = product.Detail,
                Price = product.Price,
                QuantityPr = product.QuantityPr,
                GuaranteePeriod = product.GuaranteePeriod,
                SupplierId = product.SupplierId,
                CategoryId = product.CategoryId,
                SupplierName = product.Supplier?.SupplierName,
                CategoryName = product.Category?.CategoryName,
                Images = product.Images.Select(i => i.ImageHref).ToArray()
            };
        }

        public async Task<FilteredProductResponse> GetFilteredProductsAsync(Filter filter)
        {
            var query = _appDbContext.Product
                .Include(p => p.Supplier)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => !p.IsDeleted)
                .AsQueryable();

            query = query.Where(p => p.Price >= (ulong)filter.MinPrice);

            query = query.Where(p => p.Price <= (ulong)filter.MaxPrice);

            if (!string.IsNullOrEmpty(filter.SearchKey))
            {
                var searchKeyLower = filter.SearchKey.ToLower();
                query = query.Where(p => p.NamePr.ToLower().Contains(searchKeyLower) ||
                                         p.NameSerial.ToLower().Contains(searchKeyLower) ||
                                         p.Detail.ToLower().Contains(searchKeyLower) ||
                                         p.Category.CategoryName.ToLower().Contains(searchKeyLower));
            }

            if (!string.IsNullOrEmpty(filter.SupplierId))
            {
                query = query.Where(p => p.SupplierId == filter.SupplierId);
            }

            if (!string.IsNullOrEmpty(filter.CategoryId))
            {
                query = query.Where(p => p.CategoryId == filter.CategoryId);
            }

            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                switch (filter.SortBy.ToLower())
                {
                    case "name":
                        query = filter.IsDescending ?
                            query.OrderByDescending(p => p.NamePr) :
                            query.OrderBy(p => p.NamePr);
                        break;
                    case "price":
                        query = filter.IsDescending ?
                            query.OrderByDescending(p => p.Price) :
                            query.OrderBy(p => p.Price);
                        break;
                    default:
                        break;
                }
            }

            int pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            int pageSize = filter.PageSize < 1 ? 12 : filter.PageSize;

            var totalProductCount = await query.CountAsync();

            var products = await query.Skip((pageNumber - 1) * pageSize)
                                      .Take(pageSize)
                                      .Select(p => new ProductDTO
                                      {
                                          ProductId = p.ProductId,
                                          NamePr = p.NamePr,
                                          NameSerial = p.NameSerial,
                                          Detail = p.Detail,
                                          Price = p.Price,
                                          QuantityPr = p.QuantityPr,
                                          GuaranteePeriod = p.GuaranteePeriod,
                                          SupplierId = p.SupplierId,
                                          CategoryId = p.CategoryId,
                                          Images = p.Images.Select(i => i.ImageHref).ToArray()
                                      })
                                      .ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalProductCount / pageSize);
            if (totalPages < 1)
            {
                totalPages = 1;
            }
            return new FilteredProductResponse
            {
                Products = products,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalProducts = totalProductCount
            };
        }

        public async Task<List<ProductBySearchQueryModel>> GetProductBySearchQuery(string keyword)
        {
            var products = _appDbContext.Product.AsNoTracking().
                    Include(p => p.Supplier).
                    Include(p => p.Category).
                    Include(p => p.Images).
                    Where(
                        p => p.NamePr.ToLower().Contains(keyword) ||
                        p.ProductId.ToLower().Contains(keyword) ||
                        p.NameSerial.ToLower().Contains(keyword) ||
                        p.Detail.ToLower().Contains(keyword) ||
                        p.Category.CategoryName.ToLower().Contains(keyword)
                    ).
                    Select(p => new ProductBySearchQueryModel
                    {
                        Product = new Product
                        {
                            ProductId = p.ProductId,
                            NamePr = p.NamePr,
                            NameSerial = p.NameSerial,
                            Detail = p.Detail,
                            Price = p.Price,
                            QuantityPr = p.QuantityPr,
                            GuaranteePeriod = p.GuaranteePeriod,
                            SupplierId = p.SupplierId
                        },
                        Supplier = new SupplierModel
                        {
                            SupplierId = p.Supplier.SupplierId,
                            SupplierName = p.Supplier.SupplierName
                        },
                        Category = new CategoryModel
                        {
                            CategoryId = p.Category.CategoryId,
                            CategoryName = p.Category.CategoryName
                        },
                        Image = p.Images.FirstOrDefault()
                    }).Distinct().Take(6).ToList();

            return products;
        }

        public async Task DeleteProductAsync(string productId)
        {
            var product = await _appDbContext.Product
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product != null)
            {
                product.IsDeleted = true;
                await _appDbContext.SaveChangesAsync();
            }
        }

        public async Task<List<Image>> GetProductImagesAsync(string productId)
        {
            return await _appDbContext.Image
                .Where(i => i.ProductId == productId)
                .Select(i => new Image
                {
                    ImageId = i.ImageId,
                    ProductId = i.ProductId,
                    ImageHref = i.ImageHref
                })
                .ToListAsync();
        }

        public async Task AddImagesAsync(Image image)
        {
            _appDbContext.Image.Add(image);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(ProductDTO product)
        {
            var existingProduct = await _appDbContext.Product
                .FirstOrDefaultAsync(p => p.ProductId == product.ProductId && !p.IsDeleted);

            if (existingProduct != null)
            {
                existingProduct.NamePr = product.NamePr;
                existingProduct.NameSerial = product.NameSerial;
                existingProduct.Detail = product.Detail;
                existingProduct.Price = product.Price;
                existingProduct.QuantityPr = product.QuantityPr;
                existingProduct.GuaranteePeriod = product.GuaranteePeriod;
                existingProduct.SupplierId = product.SupplierId;
                existingProduct.CategoryId = product.CategoryId;

                await _appDbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteImageAsync(string productId)
        {
            var images = await _appDbContext.Image.Where(i => i.ProductId == productId).ToListAsync();

            for (int i = 0; i < images.Count; i++)
            {
                await ImageHelper.DeleteImage(images[i].ImageHref, _cloudinary);
            }
            
            if (images != null)
            {
                _appDbContext.Image.RemoveRange(images);
                await _appDbContext.SaveChangesAsync();
            }
        }
    }
}
