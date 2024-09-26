using AutoMapper;
using Microsoft.EntityFrameworkCore;
using tech_project_back_end.Data;
using tech_project_back_end.DTO;
using tech_project_back_end.Models;
using tech_project_back_end.Repository.IRepository;

namespace tech_project_back_end.Repository
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<SupplierRepository> _logger;
        private readonly IMapper _mapper;
        public SupplierRepository(AppDbContext appDbContext, ILogger<SupplierRepository> logger, IMapper mapper) { 
            this._appDbContext = appDbContext;
            this._logger = logger;
            this._mapper = mapper;
        }
        public async Task<Supplier> Create(SupplierDTO dto)
        {
            var supplier = _mapper.Map<Supplier>(dto);
            await _appDbContext.Supplier.AddAsync(supplier);
            await _appDbContext.SaveChangesAsync();
            return supplier;
        }

        public async Task<Supplier> Delete(string id)
        {
            var isExit = await _appDbContext.Supplier.FirstOrDefaultAsync(supplier => supplier.SupplierId == id);
            if (isExit == null)
            {
                _logger.LogError("Supplier not found");
                return null;
            }
            _appDbContext.Supplier.RemoveRange(isExit);
            await _appDbContext.SaveChangesAsync();
            return isExit;
        }

        public async Task<IEnumerable<Supplier>> GetAll()
        {
            return await _appDbContext.Supplier.ToListAsync();
        }

        public async Task<Supplier> GetById(string id)
        {
            return await _appDbContext.Supplier
                .FirstOrDefaultAsync(supplier => supplier.SupplierId == id);
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public async Task<Supplier> Update(SupplierDTO dto)
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

            return isExit;
        }
    }
}
