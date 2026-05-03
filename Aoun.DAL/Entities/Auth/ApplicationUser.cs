using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
namespace Aoun.DAL.Entities;
public class ApplicationUser : IdentityUser {
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserType UserType { get; set; }
    public bool IsDeleted { get; set; } = false;
    public CharityProfile? CharityProfile { get; set; }
    public DonorProfile? DonorProfile { get; set; }
    public ICollection<Zakat>? ZakatCalculations { get; set; }
}







