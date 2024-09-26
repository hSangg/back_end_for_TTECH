using tech_project_back_end.DTO;
using tech_project_back_end.Models;

namespace tech_project_back_end.Repository.IRepository
{
    public interface ISupplierRepository
    {
        Task<IEnumerable<Supplier>> GetAll();
        Task<Supplier> GetById(string id);
        Task<Supplier> Create(SupplierDTO dto);
        Task<Supplier> Update(SupplierDTO dto);
        Task<Supplier> Delete(string id);
        void Save();
    }
}
