using Aoun.BLL.DTOs.Zakat;
using Aoun.BLL.Services.Zakat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using System.Threading.Tasks;

namespace Aoun.API.Controllers.Zakat
{
    [ApiController]
    [Route("api/zakat")]
    public class ZakatController : ControllerBase
    {
        // هذه هي السطور التي كانت مفقودة وتسببت في خطأ CS0103!
        private readonly ZakatService _zakat;
        private readonly PaymobService _paymob;

        public ZakatController(ZakatService zakat, PaymobService paymob)
        {
            _zakat = zakat;
            _paymob = paymob;
        }

        [HttpPost("calculate")]
        public async Task<IActionResult> Calculate(ZakatFullDto dto)
        {
            var result = await _zakat.Calculate(dto);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("pay")]
        public async Task<IActionResult> PayZakat([FromBody] PaymentRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User ID could not be extracted from the token." });
            }

            if (request.Amount <= 0)
                return BadRequest(new { message = "Amount must be greater than zero." });

            try
            {
                var checkoutUrl = await _paymob.CreateCheckoutSession(request.Amount, userId);
                return Ok(new { url = checkoutUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Payment initialization failed: " + ex.Message });
            }
        }
    }
}
