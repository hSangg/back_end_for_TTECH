using AutoMapper;
using tech_project_back_end.Data;
using tech_project_back_end.DTO;
using tech_project_back_end.Models;
using tech_project_back_end.Services.IService;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

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

        public async Task<SupplierDTO> DeleteSupplierById(string id)
        {
            var isExit =  await _appDbContext.Supplier.FirstOrDefaultAsync(supplier => supplier.SupplierId == id);
            if (isExit == null)
            {
                _logger.LogError("Supplier not found");
                return null;
            }
            _appDbContext.Supplier.RemoveRange(isExit);
            await _appDbContext.SaveChangesAsync();

            return _mapper.Map<SupplierDTO>(isExit);

        }

        public async Task<List<SupplierDTO>> GetAllSupplier()
        {
            try
            {
                var supplierList = await _appDbContext.Supplier.ToListAsync();
                var supplierDTOList = _mapper.Map<List<SupplierDTO>>(supplierList);
                return supplierDTOList;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error while adding supplier");
                return null;
            }
        }

        public async Task<SupplierDTO> GetSupplierById(string id)
        {
            var supplier = await _appDbContext.Supplier
                .FirstOrDefaultAsync(supplier => supplier.SupplierId == id);

            var supplierDTO = _mapper.Map<SupplierDTO>(supplier);
            return supplierDTO;
        }

        public async Task<SupplierDTO> UpdateSupplier(SupplierDTO dto)
        {
            var isExit = await _appDbContext.Supplier
                .FirstOrDefaultAsync(supplier => supplier.SupplierId.Equals(dto.SupplierId));
            if (isExit == null) 
            {
                _logger.LogError("Supplier is not exit");
                return null;
            }

            _mapper.Map(dto, isExit);

            await _appDbContext.SaveChangesAsync();

            return dto;
            
        }
    }
}
