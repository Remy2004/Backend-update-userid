using  Aoun.DAL.Entities;

namespace Aoun.DAL.Repositories
{
    public interface ICampaignRepository
    {
        Task<List<Campaign>> GetAllAsync();
        Task<Campaign?> GetByIdAsync(int id);
        Task AddAsync(Campaign campaign);
        void Update(Campaign campaign);
        void Delete(Campaign campaign);
        Task SaveAsync();
        IQueryable<Campaign> Query();
    }
}