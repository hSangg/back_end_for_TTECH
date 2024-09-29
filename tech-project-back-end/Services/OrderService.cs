using AutoMapper;
using tech_project_back_end.Repository.IRepository;
using tech_project_back_end.Services.IService;
using tech_project_back_end.Models;
using System.Data;
using tech_project_back_end.Data;
using tech_project_back_end.DTO.Order;
using ClosedXML.Excel;

namespace tech_project_back_end.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IMapper mapper, ILogger<OrderService> logger, IOrderRepository _orderRepository)
        {
            this._mapper = mapper;
            this._logger = logger;
            this._orderRepository = _orderRepository;
        }

        public async Task<OrderDTO> CreateOrder(OrderDTO orderDTO)
        {
            try
            {
                var order = _mapper.Map<Order>(orderDTO);
                order = await _orderRepository.Create(order);
                return _mapper.Map<OrderDTO>(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating order");
                throw;
            }
        }

        public async Task<OrderDTO> UpdateStateOrder(string id, string state)
        {
            try
            {
                var existingOrder = await _orderRepository.UpdateState(id, state);

                if (existingOrder == null)
                {
                    throw new KeyNotFoundException($"Order with ID {id} not found");
                }
                return _mapper.Map<OrderDTO>(existingOrder); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating order");
                throw;
            }
        }

        public async  Task<DataTable> GetOrderData()
        {
            DataTable dt = new DataTable();
            dt.TableName = "OrderTable";
            dt.Columns.Add("orderId", typeof(string));
            dt.Columns.Add("customerName", typeof(string));
            dt.Columns.Add("customerEmail", typeof(string));
            dt.Columns.Add("customerPhone", typeof(string));

            dt.Columns.Add("customerNote", typeof(string));
            dt.Columns.Add("deliveryFee", typeof(int));
            dt.Columns.Add("discountId", typeof(string));
            dt.Columns.Add("total", typeof(long));
            dt.Columns.Add("createdAt", typeof(DateTime));

            var orderList = await _orderRepository.GetAll();
            if (orderList.Count > 0)
            {
                orderList.ForEach(item =>
                {
                    dt.Rows.Add(item.orderId, item.customerName, item.customerEmail, item.customerPhone, item.customerNote, item.deliveryFee, item.discountId, item.total, item.createdAt);
                });
            }

            return dt;
        }

        public async Task<byte[]> GetExcelFileData()
        {
            var orderList = await this.GetOrderData();
            using (XLWorkbook wb = new())
            {
                wb.AddWorksheet(orderList, "Order Record");
                using (MemoryStream ms = new())
                {
                    wb.SaveAs(ms);

                    return ms.ToArray();
                }
            }
        }

        public async Task<IEnumerable<dynamic>> GetAllOrder()
        {
            return await _orderRepository.GetAllWithDiscountOrderByDescending();
        }

        public async Task<IEnumerable<dynamic>> GetById(string orderId)
        {
            return await _orderRepository.GetALlByIdOrderByDescending(orderId);
        }

        public async Task<IEnumerable<dynamic>> GetByUserId(string userId)
        {
            return await _orderRepository.GetByUserId(userId);
        }
    }
}


