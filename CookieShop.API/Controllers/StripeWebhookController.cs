
using CookieShop.Domain.Const;
using CookieShop.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace CookieShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StripeWebhookController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<StripeWebhookController> _logger;

        public StripeWebhookController(
            AppDbContext context, 
            IConfiguration configuration,
            ILogger<StripeWebhookController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Handle()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _configuration["Stripe:WebhookSecret"]
            );

            _logger.LogInformation("Received Stripe event: {EventId} - {EventType}", 
                stripeEvent.Id, stripeEvent.Type);

            if (await _context.WebhookEvents
                .AnyAsync(w => w.StripeEventId == stripeEvent.Id))
            {
                _logger.LogWarning("Duplicate event received: {EventId}", stripeEvent.Id);
                return Ok();
            }

            try
            {
                if (stripeEvent.Type == "payment_intent.succeeded")
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

                    if (paymentIntent != null)
                    {
                        var order = await _context.Orders
                            .FirstOrDefaultAsync(o => o.PaymentIntentId == paymentIntent.Id);

                        if (order != null)
                        {
                            if (paymentIntent.Amount == (long)(order.TotalAmount * 100))
                            {
                                order.Status = OrderStatus.Paid;

                                _context.Payments.Add(new Payment
                                {
                                    OrderId = order.Id,
                                    StripePaymentIntentId = paymentIntent.Id,
                                    Amount = paymentIntent.Amount / 100m,
                                    Status = PaymentStatus.Succeeded,
                                    PaidAt = DateTime.UtcNow
                                });

                                _logger.LogInformation("Payment succeeded for order: {OrderId}", order.Id);
                            }
                            else
                            {
                                _logger.LogWarning("Amount mismatch for order {OrderId}. Expected: {Expected}, Got: {Got}",
                                    order.Id, order.TotalAmount * 100, paymentIntent.Amount);
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Order not found for PaymentIntent: {PaymentIntentId}", paymentIntent.Id);
                        }
                    }
                }
                else if (stripeEvent.Type == "payment_intent.payment_failed")
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

                    if (paymentIntent != null)
                    {
                        var order = await _context.Orders
                            .FirstOrDefaultAsync(o => o.PaymentIntentId == paymentIntent.Id);

                        if (order != null)
                        {
                            order.Status = OrderStatus.Cancelled;

                            _context.Payments.Add(new Payment
                            {
                                OrderId = order.Id,
                                StripePaymentIntentId = paymentIntent.Id,
                                Amount = paymentIntent.Amount / 100m,
                                Status = PaymentStatus.Failed
                            });

                            _logger.LogWarning("Payment failed for order: {OrderId}, Reason: {Reason}", 
                                order.Id, paymentIntent.LastPaymentError?.Message ?? "Unknown");
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Stripe event: {EventId}", stripeEvent.Id);
                throw;
            }

            _context.WebhookEvents.Add(new WebhookEvent
            {
                StripeEventId = stripeEvent.Id,
                Processed = true
            });

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
