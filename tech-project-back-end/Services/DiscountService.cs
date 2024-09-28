using AutoMapper;
using tech_project_back_end.DTO.Discount;
using tech_project_back_end.Models;
using tech_project_back_end.Repository.IRepository;
using tech_project_back_end.Services.IService;

namespace tech_project_back_end.Services;

public class DiscountService : IDiscountService
{
    private readonly IDiscountRepository _discountRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<DiscountService> _logger;

    public DiscountService(IDiscountRepository discountRepository, IMapper mapper, ILogger<DiscountService> logger)
    {
        _discountRepository = discountRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<DiscountDTO>> GetAllDiscountsAsync()
    {
        try
        {
            var result = await _discountRepository.GetAllAsync();
            var discounts = _mapper.Map<List<DiscountDTO>>(result);
            return discounts;
        }
        catch (Exception err)
        {
            _logger.LogError(err, err.Message);
            throw;
        }
    }

    public async Task<DiscountDTO> GetDiscountByCurrentDateAsync(DateTime currentDate)
    {
        try
        {
            var result = await _discountRepository.GetAsync(dis => dis.DiscountDateFrom < currentDate && dis.DiscountDateTo > currentDate);
            var discount = _mapper.Map<DiscountDTO>(result);
            return discount;
        }
        catch (Exception err)
        {
            _logger.LogError(err, err.Message);
            throw;
        }
    }

    public async Task<DiscountDTO> CreateDiscountAsync(CreateDiscountDTO entity)
    {
        try
        {
            Discount discountForCreate = _mapper.Map<Discount>(entity);
            discountForCreate.DiscountId = Guid.NewGuid().ToString();
            discountForCreate.DiscountCode = Guid.NewGuid().ToString();
            var result = await _discountRepository.AddAsync(discountForCreate);
            var newDiscount = _mapper.Map<DiscountDTO>(result);
            return newDiscount;
        }
        catch (Exception err)
        {
            _logger.LogError(err, err.Message);
            throw;
        }
    }

    public async Task<DiscountDTO> UpdateDiscountAsync(DiscountDTO entity)
    {
        try
        {
            Discount discountForUpdate = _mapper.Map<Discount>(entity);
            var result = await _discountRepository.UpdateAsync(discountForUpdate);
            DiscountDTO updateDiscount = _mapper.Map<DiscountDTO>(result);
            return updateDiscount;
        }
        catch (Exception err)
        {
            _logger.LogError(err, err.Message);
            throw;
        }
    }

    public async Task DeleteDiscountByIdAsync(string discountId)
    {
        try
        {
            var discountForDelete = await _discountRepository.GetAsync(a => a.DiscountId == discountId);
            await _discountRepository.RemoveAsync(discountForDelete);
        }
        catch (Exception err)
        {
            _logger.LogError(err, err.Message);
            throw;
        }
    }

}