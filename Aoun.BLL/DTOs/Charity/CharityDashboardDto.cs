using Aoun.BLL.DTOs.Donation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoun.BLL.DTOs.Charity
{
    public class CharityDashboardDto
    {
        public decimal TotalDonations { get; set; }
        public int TotalDonors { get; set; }
        public int TotalCases { get; set; }
        public int TotalCampaigns { get; set; }
        public decimal EmergencyFundBalance { get; set; }

        public List<DonationGrowthDto> WeeklyGrowth { get; set; } = new();
        public List<DonationGrowthDto> MonthlyGrowth { get; set; } = new();

        public List<CategoryDistributionDto> CategoryDistribution { get; set; }= new ();

        public List<RecentDonationStatisticDto> RecentDonationStatistic { get; set; } = new();
        public List<TopDonorDto> TopDonors { get; set; } = new();
        public List<DonorSummaryDto> AllDonors { get; set; } = new();
        public List<DonationGrowthDto> WeeklyDonorsGrowth { get; set; } = new();
        public List<DonationGrowthDto> MonthlyDonorsGrowth { get; set; } = new();

        public decimal TodayDonations { get; set; }

        public decimal MonthlyGrowthPercent { get; set; }
        public decimal LastMonthTotal { get; set; }
        public decimal ThisMonthTotal { get; set; }

        
    }


    public class DonationGrowthDto
    {
        public string Date { get; set; }
          public decimal Amount { get; set; }
       // public int Amount { get; set; } //
    }

    public class CategoryDistributionDto
    {
        public string CategoryName { get; set; }
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class RecentDonationStatisticDto
    {
        public string DonorName { get; set; }
        public decimal Amount { get; set; }
        public string TargetName { get; set; } // Case أو Campaign
        public DateTime Date { get; set; }
       
    }


    public class TopDonorDto
    {
        public string DonorId { get; set; }
        public string DonorName { get; set; }
        public decimal TotalAmount { get; set; }
        public int DonationsCount { get; set; }
    }

    public class DonorSummaryDto
    {
        public string DonorId { get; set; }
        public string DonorName { get; set; }
        public decimal TotalAmount { get; set; }
        public int DonationsCount { get; set; }
    }

}
