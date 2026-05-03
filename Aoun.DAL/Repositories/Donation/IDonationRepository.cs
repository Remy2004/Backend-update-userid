using Aoun.DAL.Entities;
using System.Linq;
using System.Threading.Tasks;
using DonationEntity = Aoun.DAL.Entities.Donation;
using CaseEntity = Aoun.DAL.Entities.Case;

namespace Aoun.DAL.Repositories.Donation
{
    public interface IDonationRepository
    {
        Task AddAsync(DonationEntity donation);     // ده المفروض يتغير ل Donation مش DonationEntity
        Task<DonationEntity?> GetByIdAsync(int id);            // ده المفروض يتغير ل Donation مش DonationEntity
        IQueryable<DonationEntity> Query();             // ده المفروض يتغير ل IQueryable<Donation> مش DonationEntity
        void Update(DonationEntity donation);    // ده المفروض يتغير ل Donation مش DonationEntity
        Task SaveAsync();

        Task<ApplicationUser?> GetUserByIdAsync(string id);
        Task<CaseEntity?> GetCaseByIdAsync(int id);       //ايه  CaseEntity  مصدرها جاى منين والمفروض تكون Case مش CaseEntity
        Task<Campaign?> GetCampaignByIdAsync(int id);

        // 🔥 تم تحديث النوع هنا ليتطابق مع الـ Repository
        Task<CharityProfile?> GetCharityByIdAsync(int id);         //ده المفروض يتغير ل charity
    }
}