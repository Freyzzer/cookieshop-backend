namespace CookieShop.Domain.Models
{
    public class WebhookEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string StripeEventId { get; set; } = string.Empty;

        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

        public bool Processed { get; set; } = false;
    }
}
