using Aoun.BLL.DTOs.Donations;
using Aoun.BLL.DTOs.Payment;

namespace Aoun.BLL.Interfaces.Donation
{
    public interface IDonationService
    {
        Task<object> CreateDonation(DonationCreateDto dto,string? userId);
        Task<object> Pay(PaymentDto dto, string? userId);

        //Task<object> GetCaseDonations(int caseId, int page, int pageSize);
        Task<object> GetCaseDonations(int caseId, int page, int pageSize, string userId);
        Task<object> GetCampaignDonations(int campaignId, int page, int pageSize, string userId);
    }
}