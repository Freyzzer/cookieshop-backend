using CookieShop.Application.DTOs;


namespace CookieShop.Application.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto);
    }
}
