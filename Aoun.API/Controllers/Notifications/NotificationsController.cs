using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Aoun.DAL.Data;
using System.Linq;
using System.Security.Claims;

namespace Aoun.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase {
    private readonly ApplicationDbContext _db;
    public NotificationsController(ApplicationDbContext db) => _db = db;

    [HttpGet]
    public IActionResult GetMyNotifications() {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var dbNotifications = _db.Notifications.Where(n => n.UserId == userId).ToList();
        
        if (!dbNotifications.Any()) {
            return Ok(new[] {
                new { Title = "ترحيب", Message = "أهلاً بك في منصة عون!", Time = "الآن" }
            });
        }
        return Ok(dbNotifications);
    }
}