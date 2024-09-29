using System.Linq.Expressions;
using tech_project_back_end.Models;

namespace tech_project_back_end.Repository.IRepository;

public interface IDiscountRepository
{
    Task<List<Discount>> GetAll(Expression<Func<Discount, bool>>? filter = null);
    
    Task<Discount> GetDiscount(Expression<Func<Discount, bool>>? filter = null, bool tracked = true);
    
    Task<Discount> Create(Discount entity);
    
    Task<Discount> Update(Discount entity);
    
    Task Delete(Discount entity);
    
    Task SaveAsync();
    
}