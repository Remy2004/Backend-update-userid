using Aoun.BLL.DTOs.Auth;
using Aoun.BLL.DTOs.Document;
using Aoun.BLL.Interfaces.Auth; // تأكد من المسار عندك
using Aoun.DAL.Data;
using Aoun.DAL.Entities;
using Aoun.DAL.Entities.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;

namespace Aoun.BLL.Services.Charity
{
    public class RealCharityService : ICharityService
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;
        public RealCharityService(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // 1. تقديم الطلب كامل (بيانات + مستند)
        public async Task<bool> CompleteProfileAsync(CharityRegistrationDto model, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return false;

            var profile = await _db.CharityProfiles
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);

            if (profile == null)
            {
                profile = new CharityProfile
                {
                    UserId = userId,
                    CharityName = model.CharityName.Trim(),
                    LicenseNumber = model.LicenseNumber.Trim(),
                    Address = model.Address ?? "غير محدد",
                    Description = model.Description ?? "لا توجد نبذة تعريفية",
                   
                    Status = ProfileStatus.Pending,
                    CreatedAt = DateTime.UtcNow

                };

                _db.CharityProfiles.Add(profile);
            }
            else
            {
                profile.CharityName = model.CharityName.Trim();
                profile.LicenseNumber = model.LicenseNumber.Trim();
                profile.Address = model.Address ?? profile.Address;
                profile.Description = model.Description ?? profile.Description;

                
                profile.Status = ProfileStatus.Pending;
            }

            await _db.SaveChangesAsync();
            return true;
        }

        // 3. دالة استعلام الجمعية عن حالة طلبها
        public async Task<object?> GetCharityStatusAsync(string userId)
        {
            var profile = await _db.CharityProfiles
                .Where(c => c.UserId == userId && !c.IsDeleted)
                .Select(c => new
                {
                    Id = c.Id,
                    CharityName = c.CharityName,
                    Status = c.Status.ToString()
                })
                .FirstOrDefaultAsync();

            return profile;
        }

        //public Task<bool> UploadDocumentAsync(int charityId, string docUrl)
        //{
        //    throw new NotImplementedException("تم دمج هذه الدالة مع CompleteProfileAsync");
        //}


        public async Task<bool> UploadDocumentsAsync(CharityDocumentsDto dto, string userId)
        {
            var charity = await _db.CharityProfiles
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);

            if (charity == null)
                return false;

            // حذف القديم
            var oldDocs = _db.CharityDocuments
                .Where(d => d.CharityProfileId == charity.Id);

            _db.CharityDocuments.RemoveRange(oldDocs);

           
            var uploadsPath = Path.Combine(_env.WebRootPath, "Uploads/Charities", charity.Id.ToString());


            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            await SaveFile(dto.RegistrationCertificate, DocumentType.RegistrationCertificate, charity.Id, uploadsPath);
            await SaveFile(dto.TaxCard, DocumentType.TaxCard, charity.Id, uploadsPath);
            await SaveFile(dto.BankAccountProof, DocumentType.BankAccountProof, charity.Id, uploadsPath);
            await SaveFile(dto.NationalId, DocumentType.NationalId, charity.Id, uploadsPath);

            charity.Status = ProfileStatus.Pending;
            await _db.SaveChangesAsync();
            return true;
        }

        private async Task SaveFile(IFormFile file, DocumentType type, int charityId, string path)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(path, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            _db.CharityDocuments.Add(new CharityDocument
            {
                CharityProfileId = charityId,
                FileName = fileName,
                FilePath = $"/Uploads/Charities/{charityId}/{fileName}",
                DocumentType = type
            });
        }


    }
}