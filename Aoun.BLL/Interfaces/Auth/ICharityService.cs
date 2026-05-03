using System.Threading.Tasks;
using Aoun.BLL.DTOs.Auth;

namespace Aoun.BLL.Interfaces.Auth;
public interface ICharityService {
    Task<bool> CompleteProfileAsync(CharityRegistrationDto model, string userId);
    Task<bool> UploadDocumentAsync(int charityId, string docUrl);
    Task<object?> GetCharityStatusAsync(string userId);
}
