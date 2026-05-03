using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CategoryEntity = Aoun.DAL.Entities.Category.Category;
using ReportEntity = Aoun.DAL.Entities.Cases.Report;

namespace Aoun.DAL.Entities
{
    public class Case
    {
        public int Id { get; set; }

        public bool IsDeleted { get; set; } = false;

        public CaseStatus Status { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public decimal RequiredAmount { get; set; }
        public decimal CollectedAmount { get; set; }

        public bool IsUrgent { get; set; }
        public bool IsCompleted { get; set; }

        public int CategoryId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public CategoryEntity? Category { get; set; }

        // 🔥 التعديل هنا: واحدة بس لـ CharityId و واحدة بس لـ CharityProfile
        public int CharityId { get; set; }

        public CharityProfile Charity { get; set; }

        public List<Donation>? Donations { get; set; }
        public List<ReportEntity>? Reports { get; set; }
    }
}