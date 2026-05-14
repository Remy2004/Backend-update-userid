using Aoun.BLL.DTOs.Admin;
using Aoun.BLL.DTOs.Case;
using Aoun.BLL.Interfaces.Admin;
using Aoun.BLL.Interfaces.Email;
using Aoun.DAL.Data;
using Aoun.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aoun.BLL.Services.Admin
{
    public class RealAdminService : IAdminService
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailService _emailService;

        public RealAdminService(ApplicationDbContext db, IEmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }

        public async Task<object> GetStatsAsync() // خليناها object عشان تطابق الـ Interface
        {
            // 1. إجمالي المستخدمين (اللي مسجلين كمتبرعين بس لسه مدفعوش)
            // بنعد اللي في جدول اليوزرز ونوعهم متبرع
            var totalUsers = await _db.Users.CountAsync(u => u.UserType == UserType.Donor);

            // 2. إجمالي المتبرعين الفعليين (اللي دفعوا مرة واحدة على الأقل)
            // بنعد اللي ليهم Profile وتبرعاتهم أكبر من 0
            var totalDonors = await _db.DonorProfiles.CountAsync(p => p.TotalDonated > 0);

            var totalCases = await _db.Cases.CountAsync(c => !c.IsDeleted);
            var totalCharities = await _db.CharityProfiles.CountAsync();
            var approvedCharities = await _db.CharityProfiles.CountAsync(c => c.Status == ProfileStatus.Approved);
            var rejectedCharities = await _db.CharityProfiles.CountAsync(c => c.Status == ProfileStatus.Rejected);
            var suspendedCharities= await _db.CharityProfiles.CountAsync(c => c.Status == ProfileStatus.Suspended);

            // إجمالي المبالغ اللي ادفعت فعلاً في السيستم
            var totalDonationsAmount = await _db.Donations.Where(d => !d.IsDeleted).SumAsync(d => d.Amount);

            return new // رجعنا anonymous object عشان نهرب من مشكلة الـ DTO اللي مش مقروء
            {
                totalUsers,
                totalDonors,
                totalCases,
                totalCharities,
                approvedCharities,
                rejectedCharities,
                suspendedCharities,
                totalDonationsAmount

            };
        }

        public async Task<object> GetAllCharitiesAsync()
        {
            return await _db.CharityProfiles.AsNoTracking()
                .Select(c => new
                {
                    Id = c.Id,
                    CharityName = c.CharityName,
                    LicenseNumber = c.LicenseNumber,
                    Status = c.Status.ToString(),
                    Description= c.Description,
                    CreatedAt = c.CreatedAt,

                    // 🔥 إضافة الملفات بشكل صحيح
                    Documents = _db.CharityDocuments
                        .Where(doc => doc.CharityProfileId == c.Id)
                        .Select(doc => new
                        {
                            doc.DocumentType,
                            doc.FilePath
                        })
                        .ToList()
                })
                .ToListAsync();
        }
        public async Task<object?> GetCharityByIdAsync(int id)
        {
            return await _db.CharityProfiles.AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new
                {
                    Id = c.Id,
                    CharityName = c.CharityName,
                    LicenseNumber = c.LicenseNumber,
                    Status = c.Status.ToString(),
                        Description = c.Description,
                    CreatedAt = c.CreatedAt,

                    // 🔥 هنا برضه الملفات كاملة
                    Documents = _db.CharityDocuments
                        .Where(doc => doc.CharityProfileId == c.Id)
                        .Select(doc => new
                        {
                            doc.DocumentType,
                            doc.FilePath
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateStatusAsync(int id, int status)
        {
            // 1 = Approve, 2 = Reject
            var charity = await _db.CharityProfiles.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == id);

            if (charity != null)
            {
                // charity.Status = (status == 1) ? ProfileStatus.Approved : ProfileStatus.Rejected;
                charity.Status = (ProfileStatus)status;
                await _db.SaveChangesAsync();

                // إرسال الإيميل للجمعية بعد الموافقة
                if (charity.Status == ProfileStatus.Approved && charity.User?.Email != null)
                {
                    await _emailService.SendEmailAsync(
                        charity.User.Email,
                        "AOUN | طلب الانضمام",
                        "مبروك! تم قبول جمعيتكم في منصة عون."
                    );
                }
                return true;
            }
            return false;
        }

        public async Task<object> GetAllCasesAsync()
        {
            return await _db.Cases
                .AsNoTracking()
                .Where(c => !c.IsDeleted)
                .Select(c => new
                {
                    c.Id,
                    c.Title,
                    c.RequiredAmount,
                    c.CollectedAmount,
                    Status = c.CollectedAmount >= c.RequiredAmount ? "Completed" : "Active"
                })
                .ToListAsync();
        }

        public async Task<bool> DeleteCaseAsync(int id)
        {
            var entity = await _db.Cases.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
            if (entity == null) return false;

            var hasDonations = await _db.Donations.AnyAsync(d => d.CaseId == id && !d.IsDeleted);
            if (hasDonations) return false;

            entity.IsDeleted = true;
            await _db.SaveChangesAsync();
            return true;
        }

        //public async Task<object> CreateCaseAsync(CaseCreateDto dto)
        //{


        //    var imagePath = "default.png";

        //    var entity = new Case
        //    {
        //        Title = dto.Title,
        //        Description = dto.Description,
        //        ImageUrl = imagePath,
        //        RequiredAmount = dto.RequiredAmount,
        //        CollectedAmount = 0,
        //        IsUrgent = dto.IsUrgent,
        //        CategoryId = dto.CategoryId,
        //        CharityId = charityId,
        //        CreatedAt = DateTime.UtcNow
        //    };

        //    await _db.Cases.AddAsync(entity);
        //    await _db.SaveChangesAsync();
        //    return entity;
        //}

        public async Task<object> UpdateCaseAsync(int id, CaseUpdateDto dto)
        {
            var entity = await _db.Cases.FindAsync(id);
            if (entity == null || entity.IsDeleted) return new { message = "الحالة غير موجودة" };

            entity.Title = dto.Title;
            entity.Description = dto.Description;
            entity.RequiredAmount = dto.RequiredAmount;
            entity.IsUrgent = dto.IsUrgent;
            entity.CategoryId = dto.CategoryId;

            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<object> GetTopDonorsAsync()
        {
            return await _db.Donations
                .Where(d => d.UserId != null && !d.IsDeleted)
                .GroupBy(d => d.UserId)
                .Select(g => new { UserId = g.Key, Total = g.Sum(x => x.Amount) })
                .OrderByDescending(x => x.Total)
                .Take(5)
                .Join(_db.Users, d => d.UserId, u => u.Id, (d, u) => new { email = u.Email, total = d.Total })
                .ToListAsync();
        }


        //public async Task<object> GetTopDonorsAsync()
        //{
        //    var grouped = await _db.Donations
        //        .Where(d => d.UserId != null && !d.IsDeleted)
        //        .GroupBy(d => d.UserId)
        //        .Select(g => new
        //        {
        //            UserId = g.Key,
        //            Total = g.Sum(x => x.Amount)
        //        })
        //        .ToListAsync();

        //    var result = grouped
        //        .Join(_db.Users,
        //            d => d.UserId,
        //            u => u.Id,
        //            (d, u) => new
        //            {
        //                email = u.Email,
        //                total = d.Total
        //            })
        //        .OrderByDescending(x => x.total)
        //        .Take(5)
        //        .ToList();

        //    return result;
        //}





        public async Task<object> GetTopCharitiesAsync()
        {
            return await _db.CharityProfiles
                .Select(c => new
                {
                    name = c.CharityName,
                    total = _db.Cases.Where(caseItem => caseItem.CharityId == c.Id && !caseItem.IsDeleted)
                                     .Sum(caseItem => caseItem.CollectedAmount)
                })
                .OrderByDescending(x => x.total)
                .Take(5)
                .ToListAsync();
        }
    }
}