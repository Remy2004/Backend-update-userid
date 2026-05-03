namespace Aoun.BLL.DTOs.Charity
{
    public class CharityCampaignCardDto
    {                         //GetallCampaign for charity (dashboard)
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }

        public decimal RequiredAmount { get; set; }
        public decimal CollectedAmount { get; set; }

        public int DonorsCount { get; set; }

        public int DaysLeft { get; set; }
        public int? CompletedInDays { get; set; }

        public DateTime? CompletedAt { get; set; } // يظهر في المكتملة بس
    }
}
