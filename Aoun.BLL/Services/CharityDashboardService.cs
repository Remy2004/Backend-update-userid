using Aoun.BLL.DTOs.Charity;
using Aoun.BLL.Interfaces;
using Aoun.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoun.BLL.Services
{
    public class CharityDashboardService : ICharityDashboardService
    {
        private readonly ApplicationDbContext _context;

        public CharityDashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CharityDashboardDto> GetDashboardAsync(string userId)
        {
            // 1️⃣ نجيب الجمعية الخاصة باليوزر
            var charity = await _context.CharityProfiles
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);

            if (charity == null)
                throw new Exception("الجمعية غير موجودة");

            // 2️⃣ نحسب كل التبرعات اللي دخلت الجمعية
            var totalDonations = await _context.Donations
                .Where(d => d.CharityId == charity.Id
                         && d.PaymentStatus == "Paid"
                         && !d.IsDeleted)
                .SumAsync(d => (decimal?)d.Amount) ?? 0;

            var today = DateTime.Today;

            var todayDonations = await _context.Donations
                .Where(d => d.CharityId == charity.Id
                         && d.PaymentStatus == "Paid"
                         && !d.IsDeleted
                         && d.CreatedAt.Date == today)
                .SumAsync(d => (decimal?)d.Amount) ?? 0;




            // ================= Total Donors =================
            var totalDonors = await _context.Donations
                .Where(d => d.CharityId == charity.Id &&
                            d.PaymentStatus == "Paid" &&
                            !d.IsDeleted &&
                            d.UserId != null)
                .Select(d => d.UserId)
                .Distinct()
                .CountAsync();


            // ================= Total Cases =================
            var totalCases = await _context.Cases
                .Where(c => c.CharityId == charity.Id && !c.IsDeleted)
                .CountAsync();

            // ================= Total Campaigns =================
            var totalCampaigns = await _context.Campaigns
                .Where(c => c.CharityId == charity.Id)
                .CountAsync();

            var emergencyFund = charity.EmergencyFund;


            // ================= Weekly Growth (Last 7 Days) =================
            var startOfWeek = DateTime.UtcNow.Date.AddDays(-6);

            var weeklyRaw = await _context.Donations
                .Where(d => d.CharityId == charity.Id &&
                            d.PaymentStatus == "Paid" &&
                            !d.IsDeleted &&
                            d.CreatedAt >= startOfWeek)
                .GroupBy(d => d.CreatedAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Amount = g.Sum(x => x.Amount)
                })
                .ToListAsync();

            // نملأ كل الأيام حتى لو مفيش تبرعات
            var weeklyData = new List<DonationGrowthDto>();

            for (int i = 0; i < 7; i++)
            {
                var date = startOfWeek.AddDays(i);

                var dayAmount = weeklyRaw
                    .FirstOrDefault(x => x.Date == date)?.Amount ?? 0;

                weeklyData.Add(new DonationGrowthDto
                {
                    Date = date.ToString("dd MMM"),   // شكل حلو للجراف
                    Amount = dayAmount
                });
            }


            // ================= Monthly Growth (Current Month) =================
            var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

            var monthlyRaw = await _context.Donations
                .Where(d => d.CharityId == charity.Id &&
                            d.PaymentStatus == "Paid" &&
                            !d.IsDeleted &&
                            d.CreatedAt >= startOfMonth)
                .GroupBy(d => d.CreatedAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Amount = g.Sum(x => x.Amount)
                })
                .ToListAsync();

            var daysInMonth = DateTime.DaysInMonth(DateTime.UtcNow.Year, DateTime.UtcNow.Month);

            var monthlyData = new List<DonationGrowthDto>();

            for (int i = 0; i < daysInMonth; i++)
            {
                var date = startOfMonth.AddDays(i);

                var dayAmount = monthlyRaw
                    .FirstOrDefault(x => x.Date == date)?.Amount ?? 0;

                monthlyData.Add(new DonationGrowthDto
                {
                    Date = date.ToString("dd"),   // رقم اليوم بس (1,2,3..)
                    Amount = dayAmount
                });
            }

            // ================= Category Distribution (Pie Chart) =================
            // ================= Category Distribution (Cases Only) =================
            var categoryData = await _context.Donations
                .Where(d => d.CharityId == charity.Id &&
                            d.PaymentStatus == "Paid" &&
                            !d.IsDeleted &&
                            d.CaseId != null) // 🔥 ناخد تبرعات الحالات فقط
                .Include(d => d.Case)
                .ThenInclude(c => c.Category)
                .GroupBy(d => d.Case.Category.Name)
                .Select(g => new CategoryDistributionDto
                {
                    CategoryName = g.Key,
                    Amount = g.Sum(x => x.Amount)
                })
                .OrderByDescending(x => x.Amount)
                .ToListAsync();

            // ================= Recent Donations =================
            var recentDonations = await _context.Donations
     .Where(d => d.CharityId == charity.Id
              && d.PaymentStatus == "Paid"
              && !d.IsDeleted)
     .Include(d => d.User)   // 🔥 الناقص
     .Include(d => d.Case)
     .Include(d => d.Campaign)
     .OrderByDescending(d => d.CreatedAt)
     .Take(5)
     .Select(d => new RecentDonationStatisticDto
     {
         DonorName = d.User.FirstName,
         Amount = d.Amount,
         TargetName = d.CaseId != null
                         ? d.Case.Title
                         : d.Campaign.Title,
         Date = d.CreatedAt
     })
     .ToListAsync();



            var topDonors = await _context.Donations
    .Where(d => d.CharityId == charity.Id
             && d.PaymentStatus == "Paid"
             && !d.IsDeleted
             && d.UserId != null)
    .GroupBy(d => new { d.UserId, d.User.FirstName })
    .Select(g => new TopDonorDto
    {
       // DonorId = g.Key.UserId,
        DonorName = g.Key.FirstName,
        TotalAmount = g.Sum(x => x.Amount),
        DonationsCount = g.Count()
    })
    .OrderByDescending(x => x.TotalAmount)
    .Take(10)
    .ToListAsync();



            var allDonors = await _context.Donations
    .Where(d => d.CharityId == charity.Id
             && d.PaymentStatus == "Paid"
             && !d.IsDeleted
             && d.UserId != null)
    .GroupBy(d => new { d.UserId, d.User.FirstName })
    .Select(g => new DonorSummaryDto
    {
      //  DonorId = g.Key.UserId,
        DonorName = g.Key.FirstName,
        TotalAmount = g.Sum(x => x.Amount),
        DonationsCount = g.Count()
    })
    .OrderByDescending(x => x.TotalAmount)
    .ToListAsync();



            //Weekly Donors Growth

            var weeklyDonorsGrowth = await _context.Donations
    .Where(d => d.CharityId == charity.Id
             && d.PaymentStatus == "Paid"
             && !d.IsDeleted
             && d.CreatedAt >= startOfWeek)
    .GroupBy(d => d.CreatedAt.Date)
    .Select(g => new DonationGrowthDto
    {
        Date = g.Key.ToString("dd MMM"),
        Amount = g.Select(x => x.UserId).Distinct().Count()
        // 👈 عدد المتبرعين الفريدين في اليوم
    })
    .ToListAsync();



            //Monthly Donors Growth


            var monthlyDonorsGrowth = await _context.Donations
     .Where(d => d.CharityId == charity.Id
              && d.PaymentStatus == "Paid"
              && !d.IsDeleted
              && d.CreatedAt >= startOfMonth)
     .GroupBy(d => d.CreatedAt.Date)
     .Select(g => new DonationGrowthDto
     {
         Date = g.Key.ToString("dd"),
         Amount = g.Select(x => x.UserId).Distinct().Count()
     })
     .ToListAsync();



            //Monthly Growth %

            var thisMonthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var lastMonthStart = thisMonthStart.AddMonths(-1);
            var lastMonthEnd = thisMonthStart.AddDays(-1);




            ///اجمالى الشهر الحالى  var thisMonthTotal = await _context.Donations
            var thisMonthTotal = await _context.Donations
            .Where(d => d.CharityId == charity.Id
                     && d.PaymentStatus == "Paid"
                     && !d.IsDeleted
                     && d.CreatedAt >= thisMonthStart)
            .SumAsync(d => (decimal?)d.Amount) ?? 0;



            //اجمالى الشهر السابق

            var lastMonthTotal = await _context.Donations
    .Where(d => d.CharityId == charity.Id
             && d.PaymentStatus == "Paid"
             && !d.IsDeleted
             && d.CreatedAt >= lastMonthStart
             && d.CreatedAt <= lastMonthEnd)
    .SumAsync(d => (decimal?)d.Amount) ?? 0;



            //حساب النسبة

            decimal monthlyGrowthPercent = 0;

            if (lastMonthTotal > 0)
            {
                monthlyGrowthPercent = ((thisMonthTotal - lastMonthTotal) / lastMonthTotal) * 100;


            }
            //لو الشهر اللي فات = 0
            if (lastMonthTotal == 0)
            {
                monthlyGrowthPercent = thisMonthTotal > 0 ? 100 : 0;
            }

    //        ///// التبرعات حسب الفئة
    //        ///

    //        var categoryDonations = await _context.Donations
    //.Where(d => d.CharityId == charity.Id
    //         && d.PaymentStatus == "Paid"
    //         && !d.IsDeleted
    //         && d.CaseId != null)
    //.Include(d => d.Case)
    //.ThenInclude(c => c.Category)
    //.GroupBy(d => d.Case.Category.Name)
    //.Select(g => new CategoryDistributionDto
    //{
    //    CategoryName = g.Key,
    //    Amount = g.Sum(x => x.Amount)
    //})
    //.OrderByDescending(x => x.Amount)
    //.ToListAsync();



            // 3️⃣ نضيف فلوس خزنة الطوارئ
            var totalMoney = totalDonations + charity.EmergencyFund;

            // 4️⃣ نحطهم في DTO ونرجعه
            var dashboard = new CharityDashboardDto
            {
                TotalDonations = totalDonations,
                TotalDonors = totalDonors,
                TotalCases = totalCases,
                TotalCampaigns = totalCampaigns,
                EmergencyFundBalance = charity.EmergencyFund,
                WeeklyGrowth = weeklyData,
                MonthlyGrowth = monthlyData,
                CategoryDistribution = categoryData,
                RecentDonationStatistic = recentDonations,
                TodayDonations = todayDonations,
                TopDonors = topDonors,
                AllDonors = allDonors,
                WeeklyDonorsGrowth = weeklyDonorsGrowth,
                MonthlyDonorsGrowth = monthlyDonorsGrowth,
                MonthlyGrowthPercent = monthlyGrowthPercent,
                LastMonthTotal = lastMonthTotal,
                ThisMonthTotal = thisMonthTotal,
              //  CategoryDistribution = categoryDonations





            };

            return dashboard;
        }
    }
}
