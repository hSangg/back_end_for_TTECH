using AutoMapper;
using tech_project_back_end.Data;
using tech_project_back_end.DTO;
using tech_project_back_end.Models;
using tech_project_back_end.Services.IService;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using tech_project_back_end.Repository.IRepository;

namespace tech_project_back_end.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ILogger<SupplierService> _logger;

        public SupplierService(AppDbContext appDbContext, IMapper mapper, ILogger<SupplierService> logger, ISupplierRepository supplierRepository)
        {
            this._appDbContext = appDbContext;
            this._mapper = mapper;
            this._logger = logger;
            this._supplierRepository = supplierRepository;
        }

        public async Task<SupplierDTO> CreateSupplier(SupplierDTO dto)
        {
            try
            {
                var supplier = await _supplierRepository.Create(dto);
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
            var supplier = await _supplierRepository.Delete(id);
            return _mapper.Map<SupplierDTO>(supplier);
        }

        public async Task<List<SupplierDTO>> GetAllSupplier()
        {
            try
            {
                var supplierList = await _supplierRepository.GetAll();
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
            var supplier = await _supplierRepository.GetById(id);

            var supplierDTO = _mapper.Map<SupplierDTO>(supplier);
            return supplierDTO;
        }

        public async Task<SupplierDTO> UpdateSupplier(SupplierDTO dto)
        {
            var supplierDTO = await _supplierRepository.Update(dto);
            return dto;
        }
    }
}
