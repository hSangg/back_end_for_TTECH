using AutoMapper;
using tech_project_back_end.Data;
using tech_project_back_end.DTO;
using tech_project_back_end.Models;
using tech_project_back_end.Services.IService;
using Microsoft.Extensions.Logging;

namespace tech_project_back_end.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper; 
        private readonly ILogger<SupplierService> _logger; 

        public SupplierService(AppDbContext appDbContext, IMapper mapper, ILogger<SupplierService> logger)
        {
            this._appDbContext = appDbContext;
            this._mapper = mapper;
            this._logger = logger; 
        }

        public async Task<SupplierDTO> CreateSupplier(SupplierDTO dto)
        {
            try
            {
                var supplier = _mapper.Map<Supplier>(dto);
                await _appDbContext.Supplier.AddAsync(supplier);
                await _appDbContext.SaveChangesAsync();

                return _mapper.Map<SupplierDTO>(supplier);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error while adding supplier");
                return null;
            }
        }
    }
}
