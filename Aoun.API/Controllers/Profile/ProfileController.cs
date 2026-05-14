using Aoun.BLL.DTOs.Profile;
using Aoun.BLL.Interfaces.Profile;
using Aoun.DAL.Data; 
using Aoun.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using System.Security.Claims;



namespace Aoun.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profile;
    private readonly ApplicationDbContext _db; 


    public ProfileController(IProfileService profile, ApplicationDbContext db)
    {
        _profile = profile;
        _db = db;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateProfileDto model)
        => Ok(await _profile.UpdateProfileAsync(GetUserId(), model));

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePass([FromBody] ChangePasswordDto model)
        => Ok(await _profile.ChangePasswordAsync(GetUserId(), model));

    [HttpPost("upload-picture")]
    public async Task<IActionResult> Picture([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("لم يتم تحميل اى ملفات.");
        var userId = GetUserId();
        var result = await _profile.UploadProfilePictureAsync(userId, file);
        return Ok(new { imageUrl = result });
    }

    [HttpPost("favorites/add/{caseId}")]
    public async Task<IActionResult> AddFavorite(int caseId)
    {
        var userId = GetUserId();

        // 1. Verify user exists in the request
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        // 2. Check if the case exists in DB
        var caseExists = await _db.Cases.AnyAsync(c => c.Id == caseId);
        if (!caseExists) return NotFound($"Case with ID {caseId} not found.");

        // 3. Check if already favorited to prevent duplicate key error
        var alreadyExists = await _db.Favorites
            .AnyAsync(f => f.UserId == userId && f.CaseId == caseId);

        if (alreadyExists) return BadRequest("موجودة بالفعل فى المفضلة.");

        // 4. Create the object manually to ensure no navigation property issues
        var favorite = new Aoun.DAL.Entities.Favorite
        {
            UserId = userId,
            CaseId = caseId,
           // AddedAt = DateTime.UtcNow
        };

        try
        {
            _db.Favorites.Add(favorite);
            await _db.SaveChangesAsync();
            return Ok(new { message = "تم الاضافة الى المفضلة بنجاح ." });
        }
        catch (Exception ex)
        {
            // This will show you the exact error in the Output window during debugging
            return StatusCode(500, new { error = ex.Message, inner = ex.InnerException?.Message });
        }
    }

    [HttpGet("favorites")]
    public async Task<IActionResult> GetFavorites()
    {
        var userId = GetUserId();
        var favs = await _db.Favorites
            .Where(f => f.UserId == userId)
            .Include(f => f.Case)
            .Select(f => new {
                f.CaseId,
                Title = f.Case != null ? f.Case.Title : "Unknown",
                Description = f.Case != null ? f.Case.Description : ""
            })
            .ToListAsync();
        return Ok(favs);
    }

    [HttpGet("activity")]
    public async Task<IActionResult> Activity()
        => Ok(await _profile.GetActivityAsync(GetUserId()));

    
   
}