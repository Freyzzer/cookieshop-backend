namespace CookieShop.Application.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentIntentAsync(Guid orderId);
    }
}
