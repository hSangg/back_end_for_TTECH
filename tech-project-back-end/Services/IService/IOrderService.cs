using System.Data;
using tech_project_back_end.DTO.Order;

namespace tech_project_back_end.Services.IService
{
    public interface IOrderService
    {
        Task<OrderDTO> CreateOrder(OrderDTO orderDTO);
        Task<OrderDTO> UpdateStateOrder(string id, string state);
        Task<DataTable> GetOrderData();
        Task<byte[]> GetExcelFileData();
        Task<IEnumerable<dynamic>> GetAllOrder();
        Task<IEnumerable<dynamic>> GetById(string id);
        Task<IEnumerable<dynamic>> GetByUserId(string userId);
    }
}
