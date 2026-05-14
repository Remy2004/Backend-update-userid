using Aoun.BLL.DTOs.Campaign;
using Aoun.BLL.DTOs.Charity;
using Aoun.BLL.DTOs.Donations;
using Aoun.BLL.DTOs.Paged;
using Aoun.BLL.Interfaces;
using Aoun.BLL.Interfaces.Campaign;
using Aoun.DAL.Data;
using Aoun.DAL.Repositories;
using Microsoft.EntityFrameworkCore;


namespace Aoun.BLL.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly CampaignStateService _campaignStateService;
        private readonly ApplicationDbContext _context;


        public CampaignService(ICampaignRepository campaignRepository,
                        CampaignStateService campaignStateService, ApplicationDbContext context)
        {
            _campaignRepository = campaignRepository;
            _campaignStateService = campaignStateService;
            _context = context;

        }




        public async Task<IEnumerable<Campaign>> GetAllCampaigns()
        {
            await _campaignStateService.SyncCampaignStatesAsync();

            var campaigns = await _campaignRepository.GetAllAsync();
            return campaigns;
        }



        // ================= CREATE =================
        public async Task<Campaign> CreateCampaign(CampaignCreateDto dto, string userId)
        {

            var charityId = await _context.CharityProfiles
        .Where(c => c.UserId == userId)
        .Select(c => c.Id)
        .FirstOrDefaultAsync();


            var imagePath = await ImageHelper.SaveImageAsync(dto.Image, "campaigns");

            var campaign = new Campaign
            {
                Title = dto.Title,
                Description = dto.Description,
                ImageUrl = imagePath,
                RequiredAmount = dto.RequiredAmount,
                CollectedAmount = 0,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                CharityId = charityId,
                CreatedAt = DateTime.UtcNow,
                IsCompleted = false
            };

            await _campaignRepository.AddAsync(campaign);
            await _campaignRepository.SaveAsync();

            return campaign;
        }

        // ================= HOME =================
        public async Task<IEnumerable<CampaignHomeDto>> GetHomeCampaigns()
        {
            var today = DateTime.UtcNow;

            return await _campaignRepository.Query()
                .Where(c => !c.IsCompleted && c.StartDate <= today && c.EndDate >= today)
                .OrderBy(c => Guid.NewGuid())
                .Take(6)
                .Select(c => new CampaignHomeDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    ImageUrl = c.ImageUrl
                })
                .ToListAsync();
        }






        // ================= CHARITY =================
        //public async Task<object> GetCharityCampaigns(int charityId, string status, int page, int pageSize)
        //{
        //    if (page <= 0) page = 1;
        //    if (pageSize <= 0 || pageSize > 50) pageSize = 10;

        //    var today = DateTime.UtcNow;

        //    var query = _campaignRepository.Query()
        //   .Where(c => c.CharityId == charityId)
        //   .Include(c => c.Donations);

        //    if (status == "completed")
        //        query = query.Where(c => c.IsCompleted);
        //    else if (status == "active")
        //        query = query.Where(c => !c.IsCompleted);

        //    var allCampaigns = await query.ToListAsync();

        //    var stats = new CampaignStatsDto
        //    {
        //        TotalDonations = allCampaigns.Sum(c => c.CollectedAmount),
        //        CampaignsCount = allCampaigns.Count,
        //        DonorsCount = allCampaigns
        //            .SelectMany(c => c.Donations)
        //            .Select(d => d.UserId)
        //            .Distinct()
        //            .Count()
        //    };

        //    var campaigns = allCampaigns
        //        .OrderByDescending(c => c.CreatedAt)
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToList();

        //    var cards = campaigns.Select(c => new CharityCampaignCardDto
        //    {
        //        Id = c.Id,
        //        Title = c.Title,
        //        ImageUrl = c.ImageUrl,
        //        RequiredAmount = c.RequiredAmount,
        //        CollectedAmount = c.CollectedAmount,
        //        DonorsCount = c.Donations.Count,
        //        DaysLeft = c.IsCompleted ? 0 : Math.Max(0, (c.EndDate - today).Days),
        //        CompletedAt = c.CompletedAt,
        //        CompletedInDays = c.CompletedAt.HasValue
        //            ? (c.CompletedAt.Value.Date - c.StartDate.Date).Days + 1
        //            : null
        //    }).ToList();

        //    return new
        //    {
        //        Stats = stats,
        //        Page = page,
        //        PageSize = pageSize,
        //        TotalCount = allCampaigns.Count,
        //        Campaigns = cards
        //    };
        //}


        public async Task<object> GetCharityCampaigns(int charityId, string status, int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0 || pageSize > 50) pageSize = 10;

            var today = DateTime.UtcNow;

            IQueryable<Campaign> query = _campaignRepository.Query()
                .Where(c => c.CharityId == charityId)
                .Include(c => c.Donations);

            if (status == "completed")
                query = query.Where(c => c.IsCompleted);
            else if (status == "active")
                query = query.Where(c => !c.IsCompleted);

            // ✔️ مهم: نعمل count قبل التحويل لـ list
            var totalCount = await query.CountAsync();

            var campaigns = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var stats = new CampaignStatsDto
            {
                TotalDonations = campaigns.Sum(c => c.CollectedAmount),
                CampaignsCount = campaigns.Count,
                DonorsCount = campaigns
                    .SelectMany(c => c.Donations)
                    .Select(d => d.UserId)
                    .Distinct()
                    .Count()
            };

            var cards = campaigns.Select(c => new CharityCampaignCardDto
            {
                Id = c.Id,
                Title = c.Title,
                ImageUrl = c.ImageUrl,
                RequiredAmount = c.RequiredAmount,
                CollectedAmount = c.CollectedAmount,
                DonorsCount = c.Donations.Count,
                DaysLeft = c.IsCompleted ? 0 : Math.Max(0, (c.EndDate - today).Days),
                CompletedAt = c.CompletedAt,
                CompletedInDays = c.CompletedAt.HasValue
                    ? (c.CompletedAt.Value.Date - c.StartDate.Date).Days + 1
                    : null
            }).ToList();

            return new
            {
                Stats = stats,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Campaigns = cards
            };
        }





        // ================= PUBLIC =================
        public async Task<object> GetActiveCampaignsForUsers(int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0 || pageSize > 50) pageSize = 10;

            var today = DateTime.UtcNow;

            var query = _campaignRepository.Query()
                .Where(c => !c.IsCompleted &&
                            c.StartDate <= today &&
                            c.EndDate >= today);

            var totalCount = await query.CountAsync();

            var campaigns = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var data = campaigns.Select(c => new CampaignUserCardDto
            {
                Id = c.Id,
                Title = c.Title,
                ImageUrl = c.ImageUrl,
                RequiredAmount = c.RequiredAmount,
                CollectedAmount = c.CollectedAmount,
                DaysRemaining = c.IsCompleted ? 0 : Math.Max(0, (c.EndDate - today).Days)
            }).ToList();

            return new PagedResult<CampaignUserCardDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Data = data
            };
        }
        // ================= DETAILS USER =================
        public async Task<CampaignDetailsUserDto?> GetCampaignDetailsForUser(int id)
        {
            var today = DateTime.UtcNow;

            var campaign = await _campaignRepository.Query()
                .Include(c => c.Charity)
                .Include(c => c.Donations)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (campaign == null) return null;

            return new CampaignDetailsUserDto
            {
                Id = campaign.Id,
                Title = campaign.Title,
                Description = campaign.Description,
                ImageUrl = campaign.ImageUrl,
                // 🔥 تم التعديل هنا لـ CharityName
                CharityName = campaign.Charity.CharityName,
                RequiredAmount = campaign.RequiredAmount,
                CollectedAmount = campaign.CollectedAmount,
                DonorsCount = campaign.Donations.Count,
                DaysLeft = campaign.IsCompleted ? 0 : Math.Max(0, (campaign.EndDate - today).Days)
            };
        }

        // ================= DETAILS CHARITY =================
        //public async Task<CampaignDetailsCharityDto?> GetCampaignDetailsForCharity(int id)
        //{
        //    var campaign = await _campaignRepository.Query()
        //        .Include(c => c.Donations)
        //            .ThenInclude(d => d.User)
        //        .FirstOrDefaultAsync(c => c.Id == id);

        //    if (campaign == null) return null;

        //    var now = DateTime.UtcNow;

        //    var weekly = campaign.Donations
        //        .Where(d => d.CreatedAt >= now.AddDays(-7))
        //        .GroupBy(d => d.CreatedAt.Date)
        //        .Select(g => new DonationChartPointDto
        //        {
        //            Label = g.Key.ToString("dd/MM"),
        //            Amount = g.Sum(x => x.Amount)
        //        }).ToList();

        //    var monthly = campaign.Donations
        //        .Where(d => d.CreatedAt >= new DateTime(now.Year, now.Month, 1))
        //        .GroupBy(d => d.CreatedAt.Date)
        //        .Select(g => new DonationChartPointDto
        //        {
        //            Label = g.Key.ToString("dd/MM"),
        //            Amount = g.Sum(x => x.Amount)
        //        }).ToList();

        //    var lastDonations = campaign.Donations
        //        .OrderByDescending(d => d.CreatedAt)
        //        .Take(5)
        //        .Select(d => new LastDonationDto
        //        {
        //            UserName = d.User.FirstName,
        //            Amount = d.Amount,
        //            Date = d.CreatedAt
        //        }).ToList();

        //    return new CampaignDetailsCharityDto
        //    {
        //        Id = campaign.Id,
        //        Title = campaign.Title,
        //        Description = campaign.Description,
        //        ImageUrl = campaign.ImageUrl,
        //        RequiredAmount = campaign.RequiredAmount,
        //        CollectedAmount = campaign.CollectedAmount,
        //        DonorsCount = campaign.Donations.Select(d => d.UserId).Distinct().Count(),
        //        StartDate = campaign.StartDate,
        //        EndDate = campaign.EndDate,
        //        WeeklyDonations = weekly,
        //        MonthlyDonations = monthly,
        //        LastDonations = lastDonations
        //    };
        // }


        public async Task<CampaignDetailsCharityDto?> GetCampaignDetailsForCharity(int id, string userId)
        {
            var charity = await _context.CharityProfiles
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (charity == null)
                return null;

            var campaign = await _campaignRepository.Query()
                .Include(c => c.Donations)
                    .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(c => c.Id == id && c.CharityId == charity.Id);

            if (campaign == null) return null;

            var now = DateTime.UtcNow;

            var weekly = campaign.Donations
                .Where(d => d.CreatedAt >= now.AddDays(-7))
                .GroupBy(d => d.CreatedAt.Date)
                .Select(g => new DonationChartPointDto
                {
                    Label = g.Key.ToString("dd/MM"),
                    Amount = g.Sum(x => x.Amount)
                }).ToList();

            var monthly = campaign.Donations
                .Where(d => d.CreatedAt >= new DateTime(now.Year, now.Month, 1))
                .GroupBy(d => d.CreatedAt.Date)
                .Select(g => new DonationChartPointDto
                {
                    Label = g.Key.ToString("dd/MM"),
                    Amount = g.Sum(x => x.Amount)
                }).ToList();

            var lastDonations = campaign.Donations
                .OrderByDescending(d => d.CreatedAt)
                .Take(5)
                .Select(d => new LastDonationDto
                {
                    UserName = d.User.FirstName,
                    Amount = d.Amount,
                    Date = d.CreatedAt
                }).ToList();

            return new CampaignDetailsCharityDto
            {
                Id = campaign.Id,
                Title = campaign.Title,
                Description = campaign.Description,
                ImageUrl = campaign.ImageUrl,
                RequiredAmount = campaign.RequiredAmount,
                CollectedAmount = campaign.CollectedAmount,
                DonorsCount = campaign.Donations.Select(d => d.UserId).Distinct().Count(),
                StartDate = campaign.StartDate,
                EndDate = campaign.EndDate,
                WeeklyDonations = weekly,
                MonthlyDonations = monthly,
                LastDonations = lastDonations
            };
        }





        //// ================= UPDATE =================
        //public async Task<(bool Success, string Message)> UpdateCampaign(int id, UpdateCampaignDto dto)
        //{
        //    var campaign = await _campaignRepository.GetByIdAsync(id);

        //    if (campaign == null)
        //        return (false, "الحملة غير موجوده");

        //    campaign.Title = dto.Title;
        //    campaign.Description = dto.Description;
        //    campaign.RequiredAmount = dto.RequiredAmount;
        //    campaign.StartDate = dto.StartDate;
        //    campaign.EndDate = dto.EndDate;

        //    if (dto.Image != null)
        //    {
        //        campaign.ImageUrl = await ImageHelper.SaveImageAsync(dto.Image, "campaigns");
        //    }

        //    _campaignRepository.Update(campaign);
        //    await _campaignRepository.SaveAsync();

        //    return (true, "تم تحديث الحملة بنجاح");
        //}

        public async Task<(bool Success, string Message)> UpdateCampaign(int id, UpdateCampaignDto dto, string userId)
        {
            var charity = await _context.CharityProfiles
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (charity == null)
                return (false, "الجمعية غير موجودة");

            var campaign = await _campaignRepository.Query()
                .FirstOrDefaultAsync(c => c.Id == id && c.CharityId == charity.Id);

            if (campaign == null)
                return (false, "غير مصرح أو الحملة غير موجودة");

            campaign.Title = dto.Title;
            campaign.Description = dto.Description;
            campaign.RequiredAmount = dto.RequiredAmount;
            campaign.StartDate = dto.StartDate;
            campaign.EndDate = dto.EndDate;

            if (dto.Image != null)
            {
                campaign.ImageUrl = await ImageHelper.SaveImageAsync(dto.Image, "campaigns");
            }

            _campaignRepository.Update(campaign);
            await _campaignRepository.SaveAsync();

            return (true, "تم تحديث الحملة بنجاح");
        }




        //// ================= DELETE =================
        //public async Task<(bool Success, string Message)> DeleteCampaign(int id)
        //{
        //    var campaign = await _campaignRepository.GetByIdAsync(id);

        //    if (campaign == null)
        //        return (false, "الحملة غير موجودة ");

        //    if (campaign.Donations != null && campaign.Donations.Any())
        //        return (false, "لا يمكن حذف الحملة لانها تحتوى على تبرعات");

        //    _campaignRepository.Delete(campaign);
        //    await _campaignRepository.SaveAsync();

        //    return (true, "تم حذف الحملة بنجاح");
        //}

        public async Task<(bool Success, string Message)> DeleteCampaign(int id, string userId)
        {
            var charity = await _context.CharityProfiles
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (charity == null)
                return (false, "الجمعية غير موجودة");

            var campaign = await _campaignRepository.Query()
                .Include(c => c.Donations)
                .FirstOrDefaultAsync(c => c.Id == id && c.CharityId == charity.Id);

            if (campaign == null)
                return (false, "غير مصرح أو الحملة غير موجودة");

            if (campaign.Donations.Any())
                return (false, "لا يمكن حذف الحملة لأنها تحتوي على تبرعات");

            _campaignRepository.Delete(campaign);
            await _campaignRepository.SaveAsync();

            return (true, "تم حذف الحملة بنجاح");
        }




    }
} 