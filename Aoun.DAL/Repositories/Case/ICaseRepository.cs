using CaseEntity = Aoun.DAL.Entities.Case;

namespace Aoun.DAL.Repositories.Case
{
    public interface ICaseRepository
    {
        Task<List<CaseEntity>> GetAllAsync();
        Task<CaseEntity?> GetByIdAsync(int id);

        Task<List<CaseEntity>> GetHomeCasesAsync();

        Task<List<CaseEntity>> GetByCharityAsync(int charityId);

        Task AddAsync(CaseEntity entity);

        void Update(CaseEntity entity);

        void Delete(CaseEntity entity);

        Task SaveChangesAsync();

        IQueryable<CaseEntity> Query(); // مهم للـ search & filters
    }
}