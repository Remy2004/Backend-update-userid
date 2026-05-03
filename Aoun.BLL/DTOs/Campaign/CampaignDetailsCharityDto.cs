using Aoun.BLL.DTOs.Donations;

namespace Aoun.BLL.DTOs.Campaign
{
    public class CampaignDetailsCharityDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public decimal RequiredAmount { get; set; }
        public decimal CollectedAmount { get; set; }

        public int DonorsCount { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<DonationChartPointDto> WeeklyDonations { get; set; }
        public List<DonationChartPointDto> MonthlyDonations { get; set; }

        public List<LastDonationDto> LastDonations { get; set; }
    }
}
