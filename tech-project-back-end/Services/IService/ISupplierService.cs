using tech_project_back_end.DTO;

namespace tech_project_back_end.Services.IService
{
    public interface ISupplierService
    {
       Task<SupplierDTO> CreateSupplier(SupplierDTO dto);
       Task<List<SupplierDTO>> GetAllSupplier();
       Task<SupplierDTO> GetSupplierById(string id);
       Task<SupplierDTO> DeleteSupplierById(string id);
       Task<SupplierDTO> UpdateSupplier(SupplierDTO dto);
    }
}
