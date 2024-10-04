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

        public async Task<IEnumerable<dynamic>> GetOderDetailByOrderId(string id)
        {
            var orderDetails = await _appDbContext.DetailOrder.Where(od => od.OrderId == id).ToListAsync();

            var result = new List<dynamic>();

            foreach (var detail in orderDetails)
            {
                var product = _appDbContext.Product.Where(p => p.ProductId == detail.ProductId).FirstOrDefault();
                var image = _appDbContext.Image.Where(i => i.ProductId == detail.ProductId).FirstOrDefault();

                result.Add(new
                {
                    Product = product,
                    Image = image,
                    Quantity = detail.Quantity,
                    Price = detail.Price
                });
            }
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
