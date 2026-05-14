using Aoun.BLL.DTOs;
using Aoun.BLL.DTOs.Donations;
using Aoun.BLL.DTOs.Payment;
using Aoun.BLL.Interfaces;
using Aoun.BLL.Interfaces.Donation;
using Aoun.DAL.Data;
using Aoun.DAL.Repositories.Donation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Aoun.BLL.Services
{

    public class DonationService : IDonationService
    {
        private readonly IDonationRepository _repo;
        private readonly ApplicationDbContext _context;

        public DonationService(IDonationRepository repo , ApplicationDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        // ================= CREATE =================
        public async Task<object> CreateDonation(DonationCreateDto dto, string? userId)
        {
            if (dto.Amount <= 0)
                return new { message = "المبلغ لازم يكون أكبر من صفر" };

            if (dto.TargetType != "Case" &&
                dto.TargetType != "Campaign" &&
                dto.TargetType != "Charity")
                return new { message = "TargetType غير صحيح" };

            int charityId;

            // ================= CASE =================
            if (dto.TargetType == "Case")
            {
                var caseEntity = await _repo.GetCaseByIdAsync(dto.TargetId);


                if (caseEntity == null)
                    return new { message = "الحالة غير موجودة" };

                charityId = caseEntity.CharityId;
            }

            // ================= CAMPAIGN =================
            else if (dto.TargetType == "Campaign")
            {
                var campaign = await _repo.GetCampaignByIdAsync(dto.TargetId);

                if (campaign == null)
                    return new { message = "الحملة غير موجودة" };

                charityId = campaign.CharityId;
            }

            // ================= CHARITY =================
            else
            {
                var charity = await _repo.GetCharityByIdAsync(dto.TargetId);

                if (charity == null)
                    return new { message = "الجمعية غير موجودة" };

                charityId = dto.TargetId;
            }
            string donorName = dto.DonorName ?? "فاعل خير";

            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _repo.GetUserByIdAsync(userId);
                if (user != null)
                    donorName = user.FirstName;
            }

            /*

             */


            var donation = new Donation
            {
                Amount = dto.Amount,
                UserId = userId,
                DonorName = donorName,
                DonationTargetType = dto.TargetType,

                CaseId = dto.TargetType == "Case" ? dto.TargetId : null,
                CampaignId = dto.TargetType == "Campaign" ? dto.TargetId : null,

                CharityId = charityId,

                IsGift = dto.IsGift,
                GiftReceiverName = dto.GiftReceiverName,
                GiftReceiverPhone = dto.GiftReceiverPhone,
                GiftMessage = dto.GiftMessage,

                PaymentStatus = "Pending",
                PaymentMethod = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(donation);
            await _repo.SaveAsync();

            return new
            {
                donationId = donation.Id,
                message = "تم انشاء التبرع"
            };
        }

        // ================= PAY =================
        public async Task<object> Pay(PaymentDto dto, string? userId)
        {
            var donation = await _repo.GetByIdAsync(dto.DonationId);

            if (donation == null)
                return new { message = "التبرع غير موجود" };

            if (donation.PaymentStatus == "Paid")
                return new { message = "مدفوع بالفعل" };

            if (donation.UserId != userId)
                return new { message = "غير مسموح" };

            donation.PaymentStatus = "Paid";
            donation.PaymentMethod = dto.PaymentMethod;

            await UpdateTarget(donation);
            await CheckComplete(donation);

            await _repo.SaveAsync();

            return new { message = "تم الدفع بنجاح" };
        }

        //// ================= GET CASE DONATIONS =================

        //[Authorize(Roles = "Charity,Admin")]
        //public async Task<object> GetCaseDonations(int caseId, int page, int pageSize)
        //{
        //    //var query = _repo.Query()
        //    //    .Include(d => d.User)
        //    //    .Where(d => d.CaseId == caseId && d.PaymentStatus == "Paid");

        //    var query = _repo.Query()
        //.Include(d => d.User)
        //.Where(d => d.CaseId == caseId && d.PaymentStatus == "Paid");

        //    var total = await query.CountAsync();

        //    var data = await query
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize)
        //        .Select(d => new DonationListItemDto
        //        {
        //            Id = d.Id,
        //            DonorName = d.User != null ? d.User.FirstName : "فاعل خير",
        //            Amount = d.Amount,
        //            IsGift = d.IsGift,
        //            Date = d.CreatedAt
        //        })
        //        .ToListAsync();

        //    return new { total, data };
        //}


        public async Task<object> GetCaseDonations(int caseId, int page, int pageSize, string userId)
        {
            var charity = await _context.CharityProfiles
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (charity == null)
                return null;

            var caseEntity = await _context.Cases
                .FirstOrDefaultAsync(c => c.Id == caseId && c.CharityId == charity.Id);

            if (caseEntity == null)
                return null;

            var query = _repo.Query()
        .Include(d => d.User)
        .Where(d => d.CaseId == caseId && d.PaymentStatus == "Paid");

            var total = await query.CountAsync();

            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new DonationListItemDto
                {
                    Id = d.Id,
                    DonorName = d.User != null ? d.User.FirstName : "فاعل خير",
                    Amount = d.Amount,
                    IsGift = d.IsGift,
                    Date = d.CreatedAt
                })
                .ToListAsync();

            return new { total, data };
        }





        // ================= GET CAMPAIGN DONATIONS =================

        //[Authorize(Roles = "Charity,Admin")]
        //public async Task<object> GetCampaignDonations(int campaignId, int page, int pageSize)
        //{
        //    var query = _repo.Query()
        //        .Include(d => d.User)
        //        .Where(d => d.CampaignId == campaignId && d.PaymentStatus == "Paid");

        //    var total = await query.CountAsync();

        //    var data = await query
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize)
        //        .Select(d => new DonationListItemDto
        //        {
        //            Id = d.Id,
        //            DonorName = d.User != null ? d.User.FirstName : "فاعل خير",
        //            Amount = d.Amount,
        //            IsGift = d.IsGift,
        //            Date = d.CreatedAt
        //        })
        //        .ToListAsync();

        //    return new { total, data };
        //}


        public async Task<object> GetCampaignDonations(int campaignId, int page, int pageSize, string userId)
        {
            var charity = await _context.CharityProfiles
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (charity == null)
                return null;

            var campaign = await _context.Campaigns
                .FirstOrDefaultAsync(c => c.Id == campaignId && c.CharityId == charity.Id);

            if (campaign == null)
                return null;

            var query = _repo.Query()
                .Include(d => d.User)
                .Where(d => d.CampaignId == campaignId && d.PaymentStatus == "Paid");

            var total = await query.CountAsync();

            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new DonationListItemDto
                {
                    Id = d.Id,
                    DonorName = d.User != null ? d.User.FirstName : "فاعل خير",
                    Amount = d.Amount,
                    IsGift = d.IsGift,
                    Date = d.CreatedAt
                })
                .ToListAsync();

            return new { total, data };
        }




        // ================= HELPERS =================
        private async Task UpdateTarget(Donation donation)
        {
            if (donation.CaseId != null)
            {
                var c = await _repo.GetCaseByIdAsync(donation.CaseId.Value);
                if (c != null) c.CollectedAmount += donation.Amount;
            }
            else if (donation.CampaignId != null)
            {
                var camp = await _repo.GetCampaignByIdAsync(donation.CampaignId.Value);
                if (camp != null) camp.CollectedAmount += donation.Amount;
            }
            else
            {
                var charity = await _repo.GetCharityByIdAsync(donation.CharityId);
                if (charity != null) charity.EmergencyFund += donation.Amount;
            }
        }

        private async Task CheckComplete(Donation donation)
        {
            if (donation.CaseId != null)
            {
                var c = await _repo.GetCaseByIdAsync(donation.CaseId.Value);
                if (c != null && c.CollectedAmount >= c.RequiredAmount)
                    c.IsCompleted = true;
            }
            else if (donation.CampaignId != null)
            {
                var camp = await _repo.GetCampaignByIdAsync(donation.CampaignId.Value);
                if (camp != null && camp.CollectedAmount >= camp.RequiredAmount)
                    camp.IsCompleted = true;
            }
        }
    }
}