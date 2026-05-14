using Aoun.BLL.DTOs.Profile;
using Aoun.BLL.Interfaces.Profile;
using Aoun.DAL.Data;
using Aoun.DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Aoun.BLL.Services.Profile;

public class RealProfileService : IProfileService
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public RealProfileService(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<bool> UpdateProfileAsync(string userId, UpdateProfileDto model)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        user.FirstName = model.FullName?.Trim() ?? user.FirstName;
        if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
            user.PhoneNumber = model.PhoneNumber.Trim();

        if (!string.IsNullOrWhiteSpace(model.Email) && !string.Equals(user.Email, model.Email.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            var newEmail = model.Email.Trim();
            await _userManager.SetEmailAsync(user, newEmail);
            await _userManager.SetUserNameAsync(user, newEmail);
        }

        var updateResult = await _userManager.UpdateAsync(user);
        return updateResult.Succeeded;
    }

    public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto model)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        return result.Succeeded;
    }

    // Logic for saving physical file and updating DB
    public async Task<string> UploadProfilePictureAsync(string userId, IFormFile file)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return string.Empty;

        // Define upload folder
        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/profiles");
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        // Create unique file name
        var fileName = $"{userId}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(folderPath, fileName);

        // Save file to disk
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Update user record in DB (assuming your ApplicationUser has ProfilePictureUrl property)
        // user.ProfilePictureUrl = $"/uploads/profiles/{fileName}";
        // await _userManager.UpdateAsync(user);

        return $"/uploads/profiles/{fileName}";
    }

    public async Task<object> GetActivityAsync(string userId)
    {
        var donations = await _db.Donations
            .AsNoTracking()
            .Where(d => d.UserId.ToString() == userId && !d.IsDeleted) // تغيير DonorId إلى UserId
            .Include(d => d.Case)
            .Select(d => new
            {
                d.Amount,
                d.CreatedAt,
                CaseTitle = d.Case != null ? d.Case.Title : "General Donation"
            })
            .ToListAsync();

        return new { TotalDonations = donations.Count, History = donations };
    }



    //public async Task<UserActivityDto> GetActivityAsync(string userId)
    //{
    //    var donations = await _db.Donations
    //        .AsNoTracking()
    //        .Where(d => d.UserId.ToString() == userId &&
    //                    d.PaymentStatus == "Paid" &&
    //                    !d.IsDeleted)
    //        .Include(d => d.Case)
    //        .Include(d => d.Campaign)
    //        .OrderByDescending(d => d.CreatedAt)
    //        .Select(d => new UserDonationHistoryDto
    //        {
    //            Amount = d.Amount,
    //            Date = d.CreatedAt,
    //            TargetTitle =
    //                d.Case != null ? d.Case.Title :
    //                d.Campaign != null ? d.Campaign.Title :
    //                "Emergency Fund"
    //        })
    //        .ToListAsync();

    //    return new UserActivityDto
    //    {
    //        TotalDonationsCount = donations.Count,
    //        TotalDonatedAmount = donations.Sum(x => x.Amount),
    //        History = donations
    //    };
    //}


}