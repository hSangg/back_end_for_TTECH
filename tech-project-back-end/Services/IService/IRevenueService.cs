using tech_project_back_end.DTO;
using tech_project_back_end.DTO.RevenueDTO;

namespace tech_project_back_end.Services.IService
{
    public interface IRevenueService
    {
        Task<RevenueDTO> GetRevenue();
        Task<RevenueByYearDTO> GetRevenueByYear(int year);
        Task<RevenueByDayDTO> GetRevenueByDay();
    }
}
