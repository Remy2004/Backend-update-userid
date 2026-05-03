using Aoun.DAL.Data;
using Aoun.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using FavoriteEntity = Aoun.DAL.Entities.Favorite;

namespace Aoun.DAL.Repositories.Favorite
{
    public class FavoritesRepository : IFavoritesRepository
    {
        private readonly ApplicationDbContext _context;

        public FavoritesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<FavoriteEntity> Query()
            => _context.Favorites;

        public async Task<bool> CampaignExists(int id)
            => await _context.Campaigns.AnyAsync(c => c.Id == id);

        public async Task<bool> CaseExists(int id)
            => await _context.Cases.AnyAsync(c => c.Id == id);

        public async Task AddAsync(FavoriteEntity favorite)
            => await _context.Favorites.AddAsync(favorite);

        public void Remove(FavoriteEntity favorite)
            => _context.Favorites.Remove(favorite);

        public async Task<FavoriteEntity?> GetCampaignFavorite(string userId, int campaignId)
            => await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.CampaignId == campaignId);

        public async Task<FavoriteEntity?> GetCaseFavorite(string userId, int caseId)
            => await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.CaseId == caseId);

        public async Task SaveAsync()
            => await _context.SaveChangesAsync();


    }
}