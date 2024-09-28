using System.Linq.Expressions;
using tech_project_back_end.Models;

namespace tech_project_back_end.Repository.IRepository;

public interface IDiscountRepository
{
    Task<List<Discount>> GetAllAsync(Expression<Func<Discount, bool>>? filter = null);
    
    Task<Discount> GetAsync(Expression<Func<Discount, bool>>? filter = null, bool tracked = true);
    
    Task<Discount> AddAsync(Discount entity);
    
    Task<Discount> UpdateAsync(Discount entity);
    
    Task RemoveAsync(Discount entity);
    
    Task SaveAsync();
    
}