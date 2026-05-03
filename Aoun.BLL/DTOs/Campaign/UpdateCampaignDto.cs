using Microsoft.AspNetCore.Http;
namespace Aoun.BLL.DTOs.Campaign
{
    public class UpdateCampaignDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal RequiredAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IFormFile Image { get; set; }
    }
}
