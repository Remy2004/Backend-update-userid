using Aoun.BLL.DTOs.Auth;
using Aoun.BLL.Interfaces.Auth;
using Aoun.DAL.Data;
using Aoun.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aoun.BLL.Services.Charity
{
    public class RealCharityService : ICharityService // تأكد إن ICharityService فيها تعريف الدوال دي
    {
        private readonly ApplicationDbContext _db;

        public RealCharityService(ApplicationDbContext db)
        {
            _db = db;
        }

        // 1. تقديم الطلب كامل (بيانات + مستند)
        public async Task<bool> CompleteProfileAsync(CharityRegistrationDto model, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return false;

            var profile = await _db.CharityProfiles.FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);

            if (profile == null)
            {
                profile = new CharityProfile
                {
                    UserId = userId,
                    CharityName = model.CharityName.Trim(),
                    LicenseNumber = model.LicenseNumber.Trim(),
                    Address = model.Address ?? "غير محدد",
                    Status = ProfileStatus.Pending // الحالة دايماً Pending لحد ما الأدمن يوافق
                };
                _db.CharityProfiles.Add(profile);
            }
            else
            {
                // لو الجمعية بتعدل بياناتها بعد الرفض مثلاً
                profile.CharityName = model.CharityName.Trim();
                profile.LicenseNumber = model.LicenseNumber.Trim();
                profile.Address = model.Address ?? profile.Address;
                profile.Status = ProfileStatus.Pending; // ترجع Pending تاني للمراجعة
            }

            await _db.SaveChangesAsync(); // بنحفظ عشان ناخد الـ Id بتاع الجمعية

            // 2. حفظ المستند المرفق في جدول CharityDocuments
            if (!string.IsNullOrWhiteSpace(model.DocumentUrl))
            {
                // لو في مستندات قديمة لنفس الجمعية نمسحها عشان منعملش زحمة
                var oldDocs = _db.CharityDocuments.Where(d => d.CharityProfileId == profile.Id);
                _db.CharityDocuments.RemoveRange(oldDocs);

                _db.CharityDocuments.Add(new CharityDocument
                {
                    CharityProfileId = profile.Id,
                    DocumentName = "مستند التسجيل / الترخيص",
                    DocumentUrl = model.DocumentUrl.Trim()
                });
                await _db.SaveChangesAsync();
            }

            return true;
        }

        // 3. دالة جديدة عشان الجمعية تعرف حالة طلبها
        public async Task<object?> GetCharityStatusAsync(string userId)
        {
            var profile = await _db.CharityProfiles
                .Where(c => c.UserId == userId && !c.IsDeleted)
                .Select(c => new
                {
                    Id = c.Id,
                    CharityName = c.CharityName,
                    Status = c.Status.ToString() // هترجع "Pending" أو "Approved" أو "Rejected"
                })
                .FirstOrDefaultAsync();

            return profile;
        }

        public Task<bool> UploadDocumentAsync(int charityId, string docUrl)
        {
            throw new NotImplementedException("تم دمج هذه الدالة مع CompleteProfileAsync");
        }
    }
}