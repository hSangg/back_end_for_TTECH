using Microsoft.EntityFrameworkCore;
using tech_project_back_end.Data;
using tech_project_back_end.DTO;
using tech_project_back_end.Repository.IRepository;

namespace tech_project_back_end.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository (AppDbContext appDbContext, ILogger<ProductRepository> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
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
                .Join(_appDbContext.Product,
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
                                Image_Id = i.ImageId,
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
    }
}
