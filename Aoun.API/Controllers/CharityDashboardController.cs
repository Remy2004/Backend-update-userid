using Aoun.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Aoun.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Route("api/charity/dashboard")]
    public class CharityDashboardController : ControllerBase
    {
        private readonly ICharityDashboardService _dashboardService;

        public CharityDashboardController(ICharityDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [Authorize(Roles = "Charity,Admin")]   // ✔️ دي صح 100%
        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _dashboardService.GetDashboardAsync(userId);
            return Ok(result);
        }
    }
}
