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
        private readonly IMapper _mapper;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ILogger<SupplierService> _logger;

        public SupplierService(IMapper mapper, ILogger<SupplierService> logger, ISupplierRepository supplierRepository)
        {
            this._mapper = mapper;
            this._logger = logger;
            this._supplierRepository = supplierRepository;
        }

        public async Task<SupplierDTO?> CreateSupplier(SupplierDTO dto)
        {
            try
            {
                var supplier = await _supplierRepository.Create(dto);
                return supplier != null
                    ? _mapper.Map<SupplierDTO>(supplier)
                    : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating supplier");
                throw;
            }
        }

        public async Task<SupplierDTO?> DeleteSupplierById(string id)
        {
            try
            {
                var supplier = await _supplierRepository.Delete(id);
                return supplier != null
                    ? _mapper.Map<SupplierDTO>(supplier)
                    : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating supplier");
                throw;
            }
            
        }

        public async Task<List<SupplierDTO>> GetAllSupplier()
        {
            try
            {
                var supplierList = await _supplierRepository.GetAll();
                return _mapper.Map<List<SupplierDTO>>(supplierList);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error while adding supplier");
                throw;
            }
        }

        public async Task<SupplierDTO?> GetSupplierById(string id)
        {
            try
            {
                var supplier = await _supplierRepository.GetById(id);
                return supplier != null
                    ? _mapper.Map<SupplierDTO?>(supplier)
                    : null;
            }
            catch
            {
                _logger.LogError("Error while fetching supplier with id: ", id);
                throw;
            }

        }

        public async Task<SupplierDTO?> UpdateSupplier(SupplierDTO dto)
        {
            try
            {
                var supplier = await _supplierRepository.Update(dto);
                return supplier != null 
                    ? _mapper.Map<SupplierDTO>(supplier) 
                    : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating supplier with ID {SupplierId}", dto.SupplierId);
                throw;
            }
        }
    }
}
