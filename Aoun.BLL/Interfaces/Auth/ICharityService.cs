using Aoun.BLL.DTOs.Auth;
using Aoun.BLL.DTOs.Document;
using System.Threading.Tasks;

namespace Aoun.BLL.Interfaces.Auth;
public interface ICharityService {
    Task<bool> CompleteProfileAsync(CharityRegistrationDto model, string userId);
  //  Task<bool> UploadDocumentAsync(int charityId, string docUrl);
    Task<object?> GetCharityStatusAsync(string userId);
    Task<bool> UploadDocumentsAsync(CharityDocumentsDto dto, string userId);
}
