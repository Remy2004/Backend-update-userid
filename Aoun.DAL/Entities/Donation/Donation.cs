using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aoun.DAL.Entities
{
    public class Donation
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public string DonorName { get; set; }

        public string DonationTargetType { get; set; }
        [ForeignKey("CaseId")]
        public int? CaseId { get; set; }

        
        public virtual Case? Case { get; set; }

        // 🔥 التعديل هنا: واحدة بس لـ CharityProfile
        public int CharityId { get; set; }

        public CharityProfile Charity { get; set; }        //ايه السطر ده  تقريبا معمول على اساس ان البروفايل هو الجمعيه مش برضوا نفس الغلطه 

        public int? CampaignId { get; set; }
        public Campaign? Campaign { get; set; }

        public bool IsGift { get; set; }
        public string? GiftReceiverName { get; set; }
        public string? GiftReceiverPhone { get; set; }
        public string? GiftMessage { get; set; }

        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        
        public int? CategoryId { get; set; }
    }
}