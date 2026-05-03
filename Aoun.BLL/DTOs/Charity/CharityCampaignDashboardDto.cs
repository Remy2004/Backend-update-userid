using Aoun.BLL.DTOs.Campaign;

namespace Aoun.BLL.DTOs.Charity
{
    public class CharityCampaignDashboardDto
    {
        public CampaignStatsDto ActiveStats { get; set; }
        public CampaignStatsDto CompletedStats { get; set; }

        public List<CharityCampaignCardDto> ActiveCampaigns { get; set; }
        public List<CharityCampaignCardDto> CompletedCampaigns { get; set; }
    }
}
