using CookieShop.Domain.Const;

namespace CookieShop.Domain.Models
{
    public class Payment
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public string StripePaymentIntentId { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string Currency { get; set; } = "usd";

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? PaidAt { get; set; }
    }


}
