using System;
namespace Aoun.DAL.Entities;
public class Zakat {
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime CalculationDate { get; set; } = DateTime.Now;
    public bool IsDeleted { get; set; } = false;
    public string DonorId { get; set; } = string.Empty;
    public ApplicationUser? Donor { get; set; }
    public int? DonorProfileId { get; set; }
    public DonorProfile? DonorProfile { get; set; }
}







