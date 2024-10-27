using Microsoft.EntityFrameworkCore;
using tech_project_back_end.Data;
using tech_project_back_end.Models;
using tech_project_back_end.Repository.IRepository;

namespace tech_project_back_end.Repository
{
    public class DetailOrderRepository : IDetailOrderRepository
    {
        private readonly AppDbContext _appDbContext;

        public DetailOrderRepository(AppDbContext appDbContext)
        {
            this._appDbContext = appDbContext;
        }

        public async Task<dynamic> GetOderDetailByOrderId(string id)
        {
            var result = await _appDbContext.DetailOrder
                                 .Where(od => od.OrderId == id)
                                 .Select(x => new
                                 {
                                     Product = new
                                     {
                                         x.Product.ProductId,
                                         x.Product.NamePr,
                                     },
                                     Image = x.Product.Images.Select(i => new
                                     {
                                         i.ImageId,
                                         i.ImageHref
                                     }).FirstOrDefault(),
                                     Quantity = x.Quantity,
                                     Price = x.Price
                                 })
                                 .ToListAsync();
            return result;
        }

        public async Task<List<DetailOrder>> Add(List<DetailOrder> detailOrders)
        {
            foreach (var detailOrder in detailOrders)
            {
                _appDbContext.DetailOrder.Add(detailOrder);
                _appDbContext.SaveChanges();
            }
            return detailOrders;
        }
    }
}
