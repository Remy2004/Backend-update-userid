namespace Aoun.BLL.DTOs.Admin;
public class AdminDashboardStatsDto { 
    public int TotalDonors { get; set; } 
    public int TotalCases { get; set; } 
    public decimal TotalDonationsAmount { get; set; }
    public object? CategoryPercentages { get; set; }
}
