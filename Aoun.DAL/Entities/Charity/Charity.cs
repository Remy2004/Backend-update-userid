using Aoun.DAL.Entities;
using Aoun.DAL.Entities.Auth;
using System.ComponentModel.DataAnnotations.Schema;
//using UserEntity = Aoun.DAL.Entities.User;

namespace Aoun.DAL.Entities.Charity
{
    public class Charity
    {
        public int Id { get; set; }

        public string UserId { get; set; }   // 🔥 مهم جدا

      public ApplicationUser User { get; set; } // 🔥 مهم جدا
        public string Name { get; set; }

        public string LicenseNumber { get; set; } // رقم الترخيص

        public string Address { get; set; }
        public bool IsDeleted { get; set; } = false; // علشان الـ Soft Delete       //ماعرفش ليه والله بس ضفتها زى عبدالله 

        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal EmergencyFund { get; set; } = 0;// خزنة الطوارئ

       // public bool IsApproved { get; set; }    // علشان الأدمن           //عبدالله  مش ضايف دى 

         public ProfileStatus ProfileStatus { get; set; } // حالة الملف الشخصي (قيد الانتظار، موافق عليه، مرفوض)     //عبدالله ضافها بدل
                                                          // IsApproved عشان نقدر نميز بين الحالات المختلفة مش بس موافق او مرفوض
        public DateTime CreatedAt { get; set; }

      //  public UserEntity? User { get; set; }

        
        public List<Case>? Cases { get; set; }
        public List<Campaign>? Campaigns { get; set; }
            public List<Donation>? Donations { get; set; }
            public List<CharityDocument>? Documents { get; set; }
    }
    
    
}