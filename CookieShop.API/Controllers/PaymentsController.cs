using Microsoft.AspNetCore.Mvc;


namespace CookieShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController: ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentsController(IPaymentService service)
        {
            _service = service;
        }

        [HttpPost("{orderId}")]
        public async Task<IActionResult> CreateIntent(Guid orderId)
        {
            var clientSecret = await _service.CreatePaymentIntentAsync(orderId);
            return Ok(new { clientSecret });
        }

    }
}
