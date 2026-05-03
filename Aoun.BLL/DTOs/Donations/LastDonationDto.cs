using System;

namespace Aoun.BLL.DTOs.Donations
{
    public class LastDonationDto
    {
        public string? UserName { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
