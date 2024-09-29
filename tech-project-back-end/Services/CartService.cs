using AutoMapper;
using Microsoft.EntityFrameworkCore;
using tech_project_back_end.DTO.Cart;
using tech_project_back_end.Models;
using tech_project_back_end.Repository.IRepository;
using tech_project_back_end.Services.IService;

namespace tech_project_back_end.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public CartService(ICartRepository cartRepository, IMapper mapper, ILogger<CartService> logger)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<int> GetUserTotalProduct(string user_id)
    {
        try
        {
            int total = _cartRepository.GetCart(c => c.user_id == user_id).Result.Select(c => c.product_id)
                .Distinct().Count();

            return total;
        } 
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        } 
    }

    public async Task<List<CartProductModel>> GetCartProduct(string user_id)
    {
        try
        {
            var listCartProduct = await _cartRepository.GetCartProduct(user_id);
            return listCartProduct;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<CartDTO> AddToCart(ModifyCartDTO entity, string user_id)
    {
        try
        {
            var existingCart = await _cartRepository.GetCart(p => p.product_id == entity.product_id).Result.FirstOrDefaultAsync();
            var cartToCreate = new Cart()
            {
                product_id = entity.product_id,
                user_id = user_id,
            };

            if (existingCart == null)
            {
                cartToCreate.quantity = entity.quantity;
                var newCart = await _cartRepository.Create(cartToCreate);
                return _mapper.Map<CartDTO>(newCart);
            }
            else
            {
                cartToCreate.quantity = existingCart.quantity + entity.quantity;
                var result = await _cartRepository.Update(cartToCreate);
                return _mapper.Map<CartDTO>(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<CartDTO> UpdateQuantity(ModifyCartDTO entity, string user_id)
    {
        try
        {
            var cartToUpdate = new Cart()
            {
                product_id = entity.product_id,
                quantity = entity.quantity,
                user_id = user_id
            };
            var result = await _cartRepository.Update(cartToUpdate);
            return result != null ? _mapper.Map<CartDTO>(result) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task EmptyCart(string user_id)
    {
        try
        {
            var existingCart = await _cartRepository.GetCart(p => p.user_id == user_id).Result.FirstOrDefaultAsync();
            await _cartRepository.Delete(existingCart);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}