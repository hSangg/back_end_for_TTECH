using tech_project_back_end.DTO.DetailOrder;

namespace tech_project_back_end.Services.IService
{
    public interface IDetailOrderService
    {
        Task<IEnumerable<dynamic>> GetOderDetailByOrderId(string id);

        Task<List<DetailOrderDTO>> AddNewDetailOrder(List<DetailOrderDTO> detailOrderDTOs);
    }
}
