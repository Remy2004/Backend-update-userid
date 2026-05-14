using Aoun.DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;





namespace Aoun.DAL;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=db52247.public.databaseasp.net;Database=db52247;User Id=db52247;Password=7a=P!Aq5c9X?;Encrypt=False;TrustServerCertificate=True;"
        );

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}







