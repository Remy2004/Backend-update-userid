using System;
using System.Collections.Generic;
using Aoun.BLL.DTOs.Donations;


namespace Aoun.BLL.DTOs.Case
{
    public class CaseDetailsDto
    {
        
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string ImageUrl { get; set; }

            public decimal RequiredAmount { get; set; }
            public decimal CollectedAmount { get; set; }
            public double Progress { get; set; }

            public bool IsUrgent { get; set; }
            public bool IsCompleted { get; set; }

            public DateTime CreatedAt { get; set; }
            public DateTime? CompletedAt { get; set; }

            public string CategoryName { get; set; }
            public string CharityName { get; set; }

            public int DonorsCount { get; set; }

       
        public List<DonationChartPointDto> WeeklyDonations { get; set; }
        public List<DonationChartPointDto> MonthlyDonations { get; set; }
        // public List<RecentDonationDto> RecentDonations { get; set; }

        public List<LastDonationDto> LastDonations { get; set; }
    }
}
