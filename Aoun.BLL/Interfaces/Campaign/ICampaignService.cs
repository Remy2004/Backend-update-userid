using Aoun.BLL.DTOs;
using Aoun.BLL.DTOs.Campaign;
using CampaignEntity = Aoun.DAL.Entities.Campaign;

namespace Aoun.BLL.Interfaces.Campaign
{
    public interface ICampaignService
    {
        Task<CampaignEntity> CreateCampaign(CampaignCreateDto dto);

        Task<IEnumerable<CampaignHomeDto>> GetHomeCampaigns();

        Task<object> GetCharityCampaigns(int charityId, string status, int page, int pageSize);

        Task<object> GetActiveCampaignsForUsers(int page, int pageSize);

        Task<CampaignDetailsUserDto?> GetCampaignDetailsForUser(int id);

        Task<CampaignDetailsCharityDto?> GetCampaignDetailsForCharity(int id);

        Task<(bool Success, string Message)> UpdateCampaign(int id, UpdateCampaignDto dto);

        Task<(bool Success, string Message)> DeleteCampaign(int id);
    }
}