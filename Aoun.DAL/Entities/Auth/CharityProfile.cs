using Aoun.DAL.Entities.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aoun.DAL.Entities
{
    public class CharityProfile
    {
        public int Id { get; set; }
        public string ?UserId { get; set; }
        public ApplicationUser ?User { get; set; }

        public string CharityName { get; set; }
        public string LicenseNumber { get; set; }

        // 🔥 تم إضافة العنوان
        public string? Address { get; set; }

        public ProfileStatus Status { get; set; }

        // 🔥 تم إضافة IsDeleted عشان الـ Soft Delete يشتغل صح
        public bool IsDeleted { get; set; } = false;
        public string Description { get; set; }      //دى النبذه 

        [Column(TypeName = "decimal(18,2)")]
        public decimal EmergencyFund { get; set; } = 0;
        //  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; }

        // هنا الـ Lists اللي بتربط الجمعية بباقي السيستم
        public List<Case>? Cases { get; set; }
        public List<Campaign>? Campaigns { get; set; }
        public List<Donation>? Donations { get; set; }
        public List<CharityDocument>? Documents { get; set; }
    }
}