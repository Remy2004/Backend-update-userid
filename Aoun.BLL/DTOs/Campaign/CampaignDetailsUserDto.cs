namespace Aoun.BLL.DTOs.Campaign
{
    public class CampaignDetailsUserDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public string CharityName { get; set; }

        public decimal RequiredAmount { get; set; }
        public decimal CollectedAmount { get; set; }

        public int DonorsCount { get; set; }
        public int DaysLeft { get; set; }
    }
}
