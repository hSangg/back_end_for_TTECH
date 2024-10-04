using tech_project_back_end.Models;
namespace tech_project_back_end.Repository.IRepository
{
    public interface IDetailOrderRepository
    {
        Task<IEnumerable<dynamic>> GetOderDetailByOrderId(string id);

        Task<List<DetailOrder>> Add(List<DetailOrder> detailOrders);
    }
}
