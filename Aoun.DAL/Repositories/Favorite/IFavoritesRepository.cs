using Aoun.DAL.Entities;
using FavoriteEntity = Aoun.DAL.Entities.Favorite;

namespace Aoun.DAL.Repositories.Favorite
{
    public interface IFavoritesRepository
    {
        IQueryable<FavoriteEntity> Query();

        Task<bool> CampaignExists(int id);
        Task<bool> CaseExists(int id);

        Task AddAsync(FavoriteEntity favorite);
        void Remove(FavoriteEntity favorite);

        Task<FavoriteEntity?> GetCampaignFavorite(string userId, int campaignId);
        Task<FavoriteEntity?> GetCaseFavorite(string userId, int caseId);

        Task SaveAsync();
    }
}