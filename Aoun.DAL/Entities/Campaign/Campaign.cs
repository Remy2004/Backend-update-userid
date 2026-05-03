using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aoun.DAL.Entities
{
    public class Campaign
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public decimal RequiredAmount { get; set; }
        public decimal CollectedAmount { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        // 🔥 هنا العلاقة الوحيدة بالجمعية (مفيش غيرها)
        public int CharityId { get; set; }

        public CharityProfile Charity { get; set; }

        public List<Donation> Donations { get; set; }
    }
}