using Aoun.BLL.DTOs.Profile;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Aoun.BLL.Interfaces.Profile;

public interface IProfileService
{
    Task<bool> UpdateProfileAsync(string userId, UpdateProfileDto model);
    Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto model);
    Task<string> UploadProfilePictureAsync(string userId, IFormFile file);
    Task<object> GetActivityAsync(string userId);
}