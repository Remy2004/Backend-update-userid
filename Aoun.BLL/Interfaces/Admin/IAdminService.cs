using Aoun.BLL.DTOs.Admin;
using Aoun.BLL.DTOs.Case;
using System.Threading.Tasks;

namespace Aoun.BLL.Interfaces.Admin
{
    public interface IAdminService
    {
        Task<object> GetStatsAsync();
        Task<object> GetAllCharitiesAsync();
        Task<object?> GetCharityByIdAsync(int id);
        Task<bool> UpdateStatusAsync(int id, int status);
        Task<object> GetAllCasesAsync();
        Task<bool> DeleteCaseAsync(int id);
       // Task<object> CreateCaseAsync(CaseCreateDto dto);
        Task<object> UpdateCaseAsync(int id, CaseUpdateDto dto);
        Task<object> GetTopDonorsAsync();
        Task<object> GetTopCharitiesAsync();
    }
}