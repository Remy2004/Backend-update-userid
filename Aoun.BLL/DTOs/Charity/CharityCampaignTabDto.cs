using Aoun.BLL.DTOs.Campaign;


namespace Aoun.BLL.DTOs.Charity
{
    public class CharityCampaignTabDto
    {
        public CampaignStatsDto Stats { get; set; }
        public List<CharityCampaignCardDto> Campaigns { get; set; }
    }
}
