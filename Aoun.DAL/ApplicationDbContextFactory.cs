using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Aoun.DAL.Data;

namespace Aoun.DAL;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext> {
    public ApplicationDbContext CreateDbContext(string[] args) {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        // اتأكد إن ده نفس الـ Connection String اللي في appsettings.json
        optionsBuilder.UseSqlServer("Server=.;Database=Aoun Charity Platform;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}







