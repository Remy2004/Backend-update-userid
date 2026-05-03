using Microsoft.EntityFrameworkCore;
using Aoun.DAL.Data;
using CaseEntity = Aoun.DAL.Entities.Case;

namespace Aoun.DAL.Repositories.Case
{
    public class CaseRepository : ICaseRepository
    {
        private readonly ApplicationDbContext _context;

        public CaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<CaseEntity> Query()
        {
            return _context.Cases
                .Include(c => c.Category)
                .Include(c => c.Charity)
                .Include(c => c.Donations);
        }

        public async Task<List<CaseEntity>> GetAllAsync()
        {
            return await Query().ToListAsync();
        }

        public async Task<CaseEntity?> GetByIdAsync(int id)
        {
            return await Query().FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<CaseEntity>> GetHomeCasesAsync()
        {
            return await _context.Cases
                .Where(c => !c.IsCompleted)
                .OrderByDescending(c => c.CreatedAt)
                .Take(6)
                .ToListAsync();
        }

        public async Task<List<CaseEntity>> GetByCharityAsync(int charityId)
        {
            return await Query()
                .Where(c => c.CharityId == charityId)
                .ToListAsync();
        }

        public async Task AddAsync(CaseEntity entity)
        {
            await _context.Cases.AddAsync(entity);
        }

        public void Update(CaseEntity entity)
        {
            _context.Cases.Update(entity);
        }

        public void Delete(CaseEntity entity)
        {
            _context.Cases.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}