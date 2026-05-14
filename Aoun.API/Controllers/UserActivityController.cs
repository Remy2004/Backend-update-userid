using Aoun.BLL.DTOs.User;
using Aoun.DAL.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Aoun.API.Controllers
{
    [ApiController]                      //ده عملناه علشان يرجع سجل التبرعات فى بروفايل اليوزر بدل ما يضرب 
    [Route("api/user/activity")]
    [Authorize]
    public class UserActivityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserActivityController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("donations-history")]
        public async Task<IActionResult> GetMyDonationsHistory()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var history = await _context.Donations
                .AsNoTracking()
                .Where(d => d.UserId.ToString() == userId &&
                            d.PaymentStatus == "Paid" &&
                            !d.IsDeleted)
                .Include(d => d.Case)
                .Include(d => d.Campaign)
                .OrderByDescending(d => d.CreatedAt)
                .Select(d => new UserDonationHistoryDto
                {
                    Amount = d.Amount,
                    Date = d.CreatedAt,
                    TargetTitle =
                        d.Case != null ? d.Case.Title :
                        d.Campaign != null ? d.Campaign.Title :
                        "Emergency Fund"
                })
                .ToListAsync();

            return Ok(history);
        }
    }
}
