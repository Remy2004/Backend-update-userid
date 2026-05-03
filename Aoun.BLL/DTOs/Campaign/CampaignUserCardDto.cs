namespace Aoun.BLL.DTOs.Campaign
{
    public class CampaignUserCardDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }

        public decimal RequiredAmount { get; set; }
        public decimal CollectedAmount { get; set; }

        public int DaysRemaining { get; set; }
    }
}
