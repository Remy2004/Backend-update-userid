using Aoun.BLL.DTOs;

namespace Aoun.BLL.Interfaces.Favorite
{
public interface IFavoritesService
{
    Task<object> AddCampaign(int campaignId, string userId);
    Task<object> RemoveCampaign(int campaignId, string userId);
    Task<object> GetFavoriteCampaigns(string userId);

    Task<object> AddCase(int caseId, string userId);
    Task<object> RemoveCase(int caseId, string userId);
    Task<object> GetFavoriteCases(string userId);
}
}