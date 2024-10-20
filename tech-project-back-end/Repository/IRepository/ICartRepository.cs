using System.Linq.Expressions;
using tech_project_back_end.Models;
using tech_project_back_end.Models.ViewModel;

namespace tech_project_back_end.Repository.IRepository;

public interface ICartRepository
{
    Task<IQueryable<Cart>> GetCart(Expression<Func<Cart, bool>>? filter = null, bool tracked = true);

    Task<List<CartProductModel>> GetCartProduct(string userId);
    
    Task<Cart> Create(Cart entity);
    
    Task<Cart> Update(Cart entity);
    
    Task Delete(Cart entity);

    Task Save();
}