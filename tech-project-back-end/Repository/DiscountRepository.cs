using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using tech_project_back_end.Data;
using tech_project_back_end.Models;
using tech_project_back_end.Repository.IRepository;

namespace tech_project_back_end.Repository;

public class DiscountRepository : IDiscountRepository
{
    private readonly AppDbContext _db;
    internal DbSet<Discount> _dbSet;

    public DiscountRepository(AppDbContext db)
    {
        _db = db;
        this._dbSet = _db.Set<Discount>();
    }

    public async Task<List<Discount>> GetAllAsync(Expression<Func<Discount, bool>>? filter = null)
    {
        IQueryable<Discount> query = _dbSet;

        if (filter != null)
        {
            query = query.Where(filter);
        }
        
        return await query.ToListAsync();
    }

    public async Task<Discount> GetAsync(Expression<Func<Discount, bool>>? filter = null, bool tracked = true)
    {
        IQueryable<Discount> query = _dbSet;

        if (!tracked)
        {
            query = query.AsNoTracking();
        }
        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task<Discount> AddAsync(Discount entity)
    {
        var newDiscount = await _dbSet.AddAsync(entity);
        await SaveAsync();
        return newDiscount.Entity;
    }

    public async Task<Discount> UpdateAsync(Discount entity)
    {
        var existingDiscount = await _dbSet.FindAsync(entity.DiscountId);

        if (existingDiscount != null)
        {
            existingDiscount.DiscountAmount = entity.DiscountAmount;
            existingDiscount.DiscountDateTo = entity.DiscountDateTo;
            _db.Entry(existingDiscount).State = EntityState.Modified;
            await SaveAsync();
        }
        
        return existingDiscount;
        
    }

    public async Task RemoveAsync(Discount entity)
    {
        _dbSet.Remove(entity);
        await SaveAsync();
    }

    public async Task SaveAsync()
    {
        await _db.SaveChangesAsync();
    }
}