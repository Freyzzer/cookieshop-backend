using CookieShop.Application.Services.Interfaces;
using CookieShop.Domain.Const;
using Stripe;

namespace CookieShop.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IAppDbContext _context;

        public PaymentService(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<string> CreatePaymentIntentAsync(Guid orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
                throw new Exception("Orden no encontrada");

            if (order.Status != OrderStatus.Pending)
                throw new Exception("La orden ya fue procesada");

            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(order.TotalAmount * 100),
                Currency = "usd",
                Metadata = new Dictionary<string, string>
        {
            { "orderId", order.Id.ToString() }
        }
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options);

            // 🔥 GUARDAMOS EL ID EN LA ORDEN
            order.PaymentIntentId = intent.Id;

            await _context.SaveChangesAsync();

            return intent.ClientSecret;
        }
    }
}
