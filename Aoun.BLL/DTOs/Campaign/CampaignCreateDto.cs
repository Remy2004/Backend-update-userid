using Microsoft.AspNetCore.Http;

namespace Aoun.BLL.DTOs.Campaign
{
    public class CampaignCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }

        public decimal RequiredAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int CharityId { get; set; } // هيتشال لما نضيف JWT
    }
}