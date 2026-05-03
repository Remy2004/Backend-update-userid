using Aoun.BLL.DTOs;
using Aoun.BLL.DTOs.Favorite;
using Aoun.BLL.Interfaces.Favorite;
using Aoun.DAL.Data;
using Aoun.DAL.Entities;
using Aoun.DAL.Repositories.Favorite;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Aoun.BLL.Services
{
    public class FavoritesService : IFavoritesService
    {
        private readonly IFavoritesRepository _repo;
   

        public FavoritesService(IFavoritesRepository repo)
        {
            _repo = repo;
        }

        // ================= CAMPAIGNS =================

        public async Task<object> AddCampaign(int campaignId, string userId)
        {
            if (!await _repo.CampaignExists(campaignId))
                return new { success = false, message = "الحملة غير موجودة" };

            var exists = await _repo.GetCampaignFavorite(userId, campaignId);
            if (exists != null)
                return new { success = true, message = "الحملة مُضافة إلى المفضلة بالفعل" };

            await _repo.AddAsync(new Aoun.DAL.Entities.Favorite
            {
                UserId = userId,
                CampaignId = campaignId
            });

            await _repo.SaveAsync();
            return new { success = true, message = "تم الاضافة الى المفضلة" };
        }

        public async Task<object> GetFavoriteCampaigns(string userId)
        {
            var data = await _repo.Query()
                .Where(f => f.UserId == userId && f.CampaignId.HasValue)
                .Include(f => f.Campaign)
                .Select(f => new FavoriteCampaignDto
                {
                    Id = f.Campaign.Id,
                    Title = f.Campaign.Title,
                    Description = f.Campaign.Description,
                    RequiredAmount = f.Campaign.RequiredAmount
                })
                .ToListAsync();

            return data;
        }

        public async Task<object> RemoveCampaign(int campaignId, string userId)
        {
            var fav = await _repo.GetCampaignFavorite(userId, campaignId);
            if (fav == null)
                return new { success = false, message = "الحملة غير موجودة" };

            _repo.Remove(fav);
            await _repo.SaveAsync();

            return new { success = true, message = "تم حذف الحملة" };
        }

        // ================= CASES =================

        public async Task<object> AddCase(int caseId, string userId)
        {
            if (!await _repo.CaseExists(caseId))
                return new { success = false, message = "الحالة غير موجودة" };

            var exists = await _repo.GetCaseFavorite(userId, caseId);
            if (exists != null)
                return new { success = true, message = " الحالة مُضافة إلى المفضلة بالفعل" };

            await _repo.AddAsync(new Aoun.DAL.Entities.Favorite
            {
                UserId = userId,
                CaseId = caseId
            });

            await _repo.SaveAsync();
            return new { success = true, message = "تم الاضافة الى المفضلة" };
        }

        public async Task<object> GetFavoriteCases(string userId)
        {
            var data = await _repo.Query()
                .Where(f => f.UserId == userId && f.CaseId.HasValue)
                .Include(f => f.Case)
                .Select(f => new FavoriteCaseDto
                {
                    Id = f.Case.Id,
                    Title = f.Case.Title,
                    Description = f.Case.Description,
                    RequiredAmount = f.Case.RequiredAmount
                })
                .ToListAsync();

            return data;
        }

        public async Task<object> RemoveCase(int caseId, string userId)
        {
            var fav = await _repo.GetCaseFavorite(userId, caseId);
            if (fav == null)
                return new { success = false, message = "الحالة غير موجوده" };

            _repo.Remove(fav);
            await _repo.SaveAsync();

            return new { success = true, message = "تم حذف الحاله " };
        }

        
    }
}