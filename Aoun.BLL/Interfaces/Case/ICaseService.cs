using Aoun.BLL.DTOs.Case;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aoun.BLL.Interfaces
{
    public interface ICaseService
    {
        Task<(IEnumerable<CaseGetAllDto> Data, int TotalCount)> GetAllCases(int? categoryId, string status, int page, int pageSize);

        Task<IEnumerable<CaseHomeCardDto>> GetHomeCases();

        Task<(CaseStatsDto Stats, IEnumerable<CaseGetAllDto> Cases, int TotalCount)> GetCharityCasesByFilter(
            int charityId, string status, int? categoryId, int page, int pageSize);

        Task<Case> CreateCase(CaseCreateDto dto);

        Task<PublicCaseDetailsDto?> GetPublicCaseDetails(int id);

        Task<CaseDetailsDto?> GetCaseDetails(int id);  //دى للجمعيه 


        Task<(bool Success, string Message, CaseUpdatedResponseDto? Data)> UpdateCase(int id, CaseUpdateDto dto);

        Task<(bool Success, string Message, Case? DeletedCase)> DeleteCase(int id);



        Task<(IEnumerable<CaseGetAllDto> Data, int TotalCount)> SearchCases(
    int? categoryId = null,
    string? status = "all",
    string? keyword = null,
    string? charityName = null,
    int page = 1,
    int pageSize = 10);


    }
}