using Aoun.DAL.Data;
using Aoun.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using DonationEntity = Aoun.DAL.Entities.Donation;
using CaseEntity = Aoun.DAL.Entities.Case;


namespace Aoun.DAL.Repositories.Donation
{
    public class DonationRepository : IDonationRepository
    {
        private readonly ApplicationDbContext _context;

        public DonationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(DonationEntity donation)
        {
            await _context.Donations.AddAsync(donation);
        }

        public async Task<DonationEntity?> GetByIdAsync(int id)
        {
            return await _context.Donations
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public IQueryable<DonationEntity> Query()
        {
            return _context.Donations;
        }

        public void Update(DonationEntity donation)
        {
            _context.Donations.Update(donation);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string id)
            => await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        public async Task<CaseEntity?> GetCaseByIdAsync(int id)
            => await _context.Cases.FirstOrDefaultAsync(c => c.Id == id);

        public async Task<Campaign?> GetCampaignByIdAsync(int id)
            => await _context.Campaigns.FirstOrDefaultAsync(c => c.Id == id);

        // 🔥 التعديل هنا: استخدام CharityProfile بدلاً من الكلاس القديم
        public async Task<CharityProfile?> GetCharityByIdAsync(int id)
            => await _context.CharityProfiles.FirstOrDefaultAsync(c => c.Id == id);
    }
}