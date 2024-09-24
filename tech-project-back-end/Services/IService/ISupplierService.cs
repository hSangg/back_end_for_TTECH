using tech_project_back_end.DTO;

namespace tech_project_back_end.Services.IService
{
    public interface ISupplierService
    {
       Task<SupplierDTO> CreateSupplier(SupplierDTO dto);
    }
}
