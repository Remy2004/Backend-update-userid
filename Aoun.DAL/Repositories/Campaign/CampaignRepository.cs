using Microsoft.EntityFrameworkCore;
using Aoun.DAL.Entities;
using Aoun.DAL.Data;

namespace Aoun.DAL.Repositories.Campaigns
{
    public class CampaignRepository : ICampaignRepository
    {
        private readonly ApplicationDbContext _context;

        public CampaignRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Campaign>> GetAllAsync()
        {
            return await _context.Campaigns
                .Include(c => c.Charity)
                .ToListAsync();
        }

        public async Task<Campaign?> GetByIdAsync(int id)
        {
            return await _context.Campaigns
                .Include(c => c.Charity)
                .Include(c => c.Donations)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Campaign campaign)
        {
            await _context.Campaigns.AddAsync(campaign);
        }

        public void Update(Campaign campaign)
        {
            _context.Campaigns.Update(campaign);
        }

        public void Delete(Campaign campaign)
        {
            _context.Campaigns.Remove(campaign);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public IQueryable<Campaign> Query()
        {
            return _context.Campaigns.AsQueryable();
        }
    }
}