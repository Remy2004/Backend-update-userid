using Aoun.DAL.Entities;

namespace Aoun.DAL.Entities
{
    public class Favorite
    {
        public int Id { get; set; }

        // المستخدم صاحب المفضلة
        public string UserId { get; set; }

        public int? CampaignId { get; set; }
        public Campaign? Campaign { get; set; }

        public int? CaseId { get; set; }
        public Case? Case { get; set; }
        public ApplicationUser User { get; set; }    

        // public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
