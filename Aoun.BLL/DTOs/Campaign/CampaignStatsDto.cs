namespace Aoun.BLL.DTOs.Campaign
{
    public class CampaignStatsDto           //statusDto   الاحصائيات الحملة  (dashboard)  (total donations, total campaigns, total donors)
    {
        public decimal TotalDonations { get; set; }
        public int CampaignsCount { get; set; }
        public int DonorsCount { get; set; }
    }
}
