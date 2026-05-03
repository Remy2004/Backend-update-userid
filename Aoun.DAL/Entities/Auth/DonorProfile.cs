using System.Collections.Generic;
namespace Aoun.DAL.Entities;
public class DonorProfile {
    public int Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int TotalDonated { get; set; }
    public decimal TotalDonationsAmount { get; set; }
    public bool IsDeleted { get; set; } = false;
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
    public ICollection<Zakat>? Zakats { get; set; }
}







