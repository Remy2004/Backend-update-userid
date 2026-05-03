using Aoun.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace Aoun.BLL.Services
{
    public class CampaignStateService
    {
        private readonly ApplicationDbContext _context;

        public CampaignStateService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SyncCampaignStatesAsync()
        {
            var now = DateTime.UtcNow;

            var expiredCampaigns = await _context.Campaigns
                .Where(c => !c.IsCompleted && c.EndDate <= now)
                .ToListAsync();

            foreach (var campaign in expiredCampaigns)
            {
                campaign.IsCompleted = true;
                campaign.CompletedAt = now;
            }

            await _context.SaveChangesAsync();
        }
    }
}