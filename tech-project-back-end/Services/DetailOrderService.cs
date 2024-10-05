using AutoMapper;
using tech_project_back_end.DTO.DetailOrder;
using tech_project_back_end.Services.IService;
using tech_project_back_end.Repository.IRepository;
using tech_project_back_end.DTO.Order;
using tech_project_back_end.Models;

namespace tech_project_back_end.Services
{
    public class DetailOrderService : IDetailOrderService
    {
        private readonly IMapper _mapper;
        private readonly IDetailOrderRepository _detailOrderRepository;
        private readonly ILogger<DetailOrderService> _logger;

        public DetailOrderService(IMapper mapper, ILogger<DetailOrderService> logger, IDetailOrderRepository detailOrderRepository)
        {
            this._mapper = mapper;
            this._logger = logger;
            this._detailOrderRepository = detailOrderRepository;
        }

        public async Task<IEnumerable<dynamic>> GetOderDetailByOrderId(string id)
        {
            return await _detailOrderRepository.GetOderDetailByOrderId(id);
        }

        public async Task<List<DetailOrderDTO>> AddNewDetailOrder(List<DetailOrderDTO> detailOrderDTOs)
        {
            try
            {
                List<DetailOrder> detailOrders = new List<DetailOrder>();
                foreach(var detail in detailOrderDTOs)
                {
                    detailOrders.Add(_mapper.Map<DetailOrder>(detail));
                }
                await _detailOrderRepository.Add(detailOrders);
                return detailOrderDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding new detail order");
                throw;
            }

        }
    }
}
