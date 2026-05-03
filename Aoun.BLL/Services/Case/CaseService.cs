using Aoun.BLL.DTOs.Case;
using Aoun.BLL.DTOs.Donations;
using Microsoft.EntityFrameworkCore;
using Aoun.DAL.Repositories.Case;
using Aoun.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aoun.BLL.Services
{
    public class CaseService : ICaseService
    {
        private readonly ICaseRepository _repo;

        public CaseService(ICaseRepository repo)
        {
            _repo = repo;
        }

        // ================= GET ALL =================
        public async Task<(IEnumerable<CaseGetAllDto> Data, int TotalCount)> GetAllCases(int? categoryId, string status, int page, int pageSize)
        {
            var query = _repo.Query().Include(c => c.Category).AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(c => c.CategoryId == categoryId);

            if (status == "urgent")
                query = query.Where(c => c.IsUrgent && !c.IsCompleted);
            else if (status == "completed")
                query = query.Where(c => c.IsCompleted);

            var total = await query.CountAsync();

            var data = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CaseGetAllDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description.Length > 100 ? c.Description.Substring(0, 100) + "..." : c.Description,
                    ImageUrl = c.ImageUrl,
                    RequiredAmount = c.RequiredAmount,
                    CollectedAmount = c.CollectedAmount,
                    IsUrgent = c.IsUrgent,
                    IsCompleted = c.IsCompleted,
                    CompletedAt = c.CompletedAt,
                    CategoryId = c.CategoryId,
                    CategoryName = c.Category.Name
                })
                .ToListAsync();

            return (data, total);
        }

        // ================= HOME =================
        public async Task<IEnumerable<CaseHomeCardDto>> GetHomeCases()
        {
            return await _repo.Query()
                .Where(c => !c.IsCompleted)
                .OrderBy(c => Guid.NewGuid())
                .Take(12)
                .Select(c => new CaseHomeCardDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description.Length > 80 ? c.Description.Substring(0, 80) + "..." : c.Description,
                    ImageUrl = c.ImageUrl,
                    RequiredAmount = c.RequiredAmount,
                    CollectedAmount = c.CollectedAmount,
                    IsUrgent = c.IsUrgent
                })
                .ToListAsync();
        }

        // ================= CREATE =================
        public async Task<Aoun.DAL.Entities.Case> CreateCase(CaseCreateDto dto)
        {
            var imagePath = await ImageHelper.SaveImageAsync(dto.Image, "cases");

            var entity = new Aoun.DAL.Entities.Case
            {
                Title = dto.Title,
                Description = dto.Description,
                ImageUrl = imagePath,
                RequiredAmount = dto.RequiredAmount,
                CollectedAmount = 0,
                IsUrgent = dto.IsUrgent,
                IsCompleted = false,
                CategoryId = dto.CategoryId,
                CharityId = dto.CharityId,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            return entity;
        }

        // ================= PUBLIC DETAILS =================
        public async Task<PublicCaseDetailsDto?> GetPublicCaseDetails(int id)
        {
            var c = await _repo.Query()
                .Include(x => x.Category)
                .Include(x => x.Charity)
                .Include(x => x.Donations)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (c == null) return null;

            return new PublicCaseDetailsDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                RequiredAmount = c.RequiredAmount,
                CollectedAmount = c.CollectedAmount,
                Progress = c.RequiredAmount == 0 ? 0 :
                    (double)Math.Min(100, Math.Round((c.CollectedAmount / c.RequiredAmount) * 100, 1)),
                IsUrgent = c.IsUrgent,
                IsCompleted = c.IsCompleted,
                CategoryName = c.Category.Name,
                // 🔥 التعديل هنا لـ CharityName
                CharityName = c.Charity.CharityName,
                DonorsCount = c.Donations.Count
            };
        }

        // ================= UPDATE =================
        public async Task<(bool Success, string Message, CaseUpdatedResponseDto? Data)> UpdateCase(int id, CaseUpdateDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);

            if (entity == null)
                return (false, "الحالة غير موجودة", null);

            entity.Title = dto.Title;
            entity.Description = dto.Description;
            entity.RequiredAmount = dto.RequiredAmount;
            entity.IsUrgent = dto.IsUrgent;
            entity.CategoryId = dto.CategoryId;

            if (dto.Image != null)
            {
                var path = await ImageHelper.SaveImageAsync(dto.Image, "cases");
                entity.ImageUrl = path;
            }

            await _repo.SaveChangesAsync();

            var result = new CaseUpdatedResponseDto
            {
                Id = entity.Id,
                Title = entity.Title,
                IsUrgent = entity.IsUrgent,
                RequiredAmount = entity.RequiredAmount
            };

            return (true, "تم التحديث بنجاح", result);
        }

        // ================= DELETE =================
        public async Task<(bool Success, string Message, Aoun.DAL.Entities.Case? DeletedCase)> DeleteCase(int id)
        {
            var entity = await _repo.Query()
                .Include(c => c.Donations)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity == null)
                return (false, "الحالة غير موجودة", null);

            if (entity.Donations != null && entity.Donations.Any())
                return (false, "لا يمكن حذف الحالة لانها تحتوى على تبرعات", null);

            _repo.Delete(entity);
            await _repo.SaveChangesAsync();

            return (true, "تم حذف الحالة بنجاح", entity);
        }

        // ================= SEARCH =================
        public async Task<(IEnumerable<CaseGetAllDto> Data, int TotalCount)> SearchCases(
            int? categoryId = null,
            string? status = "all",
            string? keyword = null,
            string? charityName = null,
            int page = 1,
            int pageSize = 10)
        {
            var query = _repo.Query()
                .Include(c => c.Category)
                .Include(c => c.Charity)
                .AsQueryable();

            query = categoryId.HasValue
                ? query.Where(c => c.CategoryId == categoryId)
                : query;

            query = status == "urgent"
                ? query.Where(c => c.IsUrgent && !c.IsCompleted)
                : status == "completed"
                    ? query.Where(c => c.IsCompleted)
                    : query;

            query = !string.IsNullOrEmpty(keyword)
                ? query.Where(c => c.Title.Contains(keyword) || c.Description.Contains(keyword))
                : query;

            query = !string.IsNullOrEmpty(charityName)
                // 🔥 التعديل هنا لـ CharityName
                ? query.Where(c => c.Charity.CharityName.Contains(charityName))
                : query;

            var total = await query.CountAsync();

            var data = await query
                .OrderBy(c => c.Title)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CaseGetAllDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description.Length > 100 ? c.Description.Substring(0, 100) + "..." : c.Description,
                    ImageUrl = c.ImageUrl,
                    RequiredAmount = c.RequiredAmount,
                    CollectedAmount = c.CollectedAmount,
                    IsUrgent = c.IsUrgent,
                    IsCompleted = c.IsCompleted,
                    CompletedAt = c.CompletedAt,
                    CategoryId = c.CategoryId,
                    CategoryName = c.Category.Name
                })
                .ToListAsync();

            return (data, total);
        }

        public async Task<CaseDetailsDto?> GetCaseDetails(int id)
        {
            var caseEntity = await _repo.Query()
                .Include(c => c.Category)
                .Include(c => c.Charity)
                .Include(c => c.Donations)
                .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (caseEntity == null)
                return null;

            var weekAgo = DateTime.UtcNow.AddDays(-7);

            var weekly = caseEntity.Donations
                .Where(d => d.CreatedAt >= weekAgo)
                .GroupBy(d => d.CreatedAt.Date)
                .Select(g => new DonationChartPointDto
                {
                    Label = g.Key.ToString("dd/MM"),
                    Amount = g.Sum(x => x.Amount)
                }).ToList();

            var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

            var monthly = caseEntity.Donations
                .Where(d => d.CreatedAt >= startOfMonth)
                .GroupBy(d => d.CreatedAt.Date)
                .Select(g => new DonationChartPointDto
                {
                    Label = g.Key.ToString("dd/MM"),
                    Amount = g.Sum(x => x.Amount)
                }).ToList();

            var lastDonations = caseEntity.Donations
                .OrderByDescending(d => d.CreatedAt)
                .Take(5)
                .Select(d => new LastDonationDto
                {
                    UserName = d.User != null ? d.User.UserName : "فاعل خير",
                    Amount = d.Amount,
                    Date = d.CreatedAt
                }).ToList();

            return new CaseDetailsDto
            {
                Id = caseEntity.Id,
                Title = caseEntity.Title,
                Description = caseEntity.Description,
                ImageUrl = caseEntity.ImageUrl,
                RequiredAmount = caseEntity.RequiredAmount,
                CollectedAmount = caseEntity.CollectedAmount,
                Progress = caseEntity.RequiredAmount == 0
                    ? 0
                    : (double)Math.Min(100, Math.Round((caseEntity.CollectedAmount / caseEntity.RequiredAmount) * 100, 1)),
                IsUrgent = caseEntity.IsUrgent,
                IsCompleted = caseEntity.IsCompleted,
                CreatedAt = caseEntity.CreatedAt,
                CompletedAt = caseEntity.CompletedAt,
                CategoryName = caseEntity.Category.Name,
                // 🔥 التعديل هنا لـ CharityName
                CharityName = caseEntity.Charity.CharityName,
                DonorsCount = caseEntity.Donations.Count,
                WeeklyDonations = weekly,
                MonthlyDonations = monthly,
                LastDonations = lastDonations
            };
        }


        public async Task<(CaseStatsDto Stats, IEnumerable<CaseGetAllDto> Cases, int TotalCount)> GetCharityCasesByFilter(int charityId, string status, int? categoryId, int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0 || pageSize > 50) pageSize = 10;

            var today = DateTime.UtcNow;

            var casesToComplete = await _repo.Query()
                .Where(c => !c.IsCompleted && c.CollectedAmount >= c.RequiredAmount)
                .ToListAsync();

            foreach (var c in casesToComplete)
            {
                c.IsCompleted = true;
                c.CompletedAt = today;
            }

            await _repo.SaveChangesAsync();

            var query = _repo.Query()
                .Where(c => c.CharityId == charityId)
                .Include(c => c.Donations)
                .Include(c => c.Category)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(c => c.CategoryId == categoryId);

            query = status == "urgent"
                ? query.Where(c => c.IsUrgent && !c.IsCompleted)
                : status == "completed"
                    ? query.Where(c => c.IsCompleted)
                    : query.Where(c => !c.IsCompleted);

            var totalCount = await query.CountAsync();

            var cases = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var stats = new CaseStatsDto
            {
                TotalDonations = cases.Sum(c => c.CollectedAmount),
                CasesCount = cases.Count,
                DonorsCount = cases
                    .SelectMany(c => c.Donations)
                    .Select(d => d.UserId)
                    .Distinct()
                    .Count()
            };

            var cards = cases.Select(c => new CaseGetAllDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description.Length > 100
                    ? c.Description.Substring(0, 100) + "..."
                    : c.Description,
                ImageUrl = c.ImageUrl,
                RequiredAmount = c.RequiredAmount,
                CollectedAmount = c.CollectedAmount,
                IsUrgent = c.IsUrgent,
                IsCompleted = c.IsCompleted,
                CompletedAt = c.CompletedAt,
                CategoryId = c.CategoryId,
                CategoryName = c.Category.Name
            }).ToList();

            return (stats, cards, totalCount);
        }
    }
}