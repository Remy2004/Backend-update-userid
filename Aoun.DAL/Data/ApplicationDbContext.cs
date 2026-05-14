using Aoun.DAL.Entities;
using Aoun.DAL.Entities.Auth;
using Aoun.DAL.Entities.Cases;
using Aoun.DAL.Entities.Category;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Aoun.DAL.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

   
    public DbSet<DonorProfile> DonorProfiles { get; set; }
    public DbSet<CharityProfile> CharityProfiles { get; set; } // ✅ الجدول الأساسي المعتمد للجمعيات
    public DbSet<Case> Cases { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Donation> Donations { get; set; }
    // public DbSet<CharityDocument> CharityDocuments { get; set; }   
    public DbSet<CharityDocument> CharityDocuments { get; set; }
    public DbSet<Zakat> ZakatCalculations { get; set; }
    public DbSet<TrustScore> TrustScores { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<Category> Categories { get; set; }
    
    public DbSet<Favorite> Favorites { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // =================================================================
        // 🚨 القفل النهائي للعلاقات (عشان نمنع EF Core من اختراع CharityId1) 🚨
        // =================================================================

        // 1. ربط الجمعية (CharityProfile) بالحملات (Campaigns)
        builder.Entity<CharityProfile>()
            .HasMany(cp => cp.Campaigns)
            .WithOne(c => c.Charity)
            .HasForeignKey(c => c.CharityId)
            .OnDelete(DeleteBehavior.Restrict);

        // 2. ربط الجمعية بالحالات (Cases)
        builder.Entity<CharityProfile>()
            .HasMany(cp => cp.Cases)
            .WithOne(c => c.Charity)
            .HasForeignKey(c => c.CharityId)
            .OnDelete(DeleteBehavior.Restrict);

        // 3. ربط الجمعية بالتبرعات (Donations)
        builder.Entity<CharityProfile>()
            .HasMany(cp => cp.Donations)
            .WithOne(d => d.Charity)
            .HasForeignKey(d => d.CharityId)
            .OnDelete(DeleteBehavior.Restrict);

        // 4. ربط الحملات بالتبرعات
        builder.Entity<Campaign>()
            .HasMany(c => c.Donations)
            .WithOne(d => d.Campaign)
            .HasForeignKey(d => d.CampaignId)
            .OnDelete(DeleteBehavior.Restrict);

        // 5. ربط الحالات بالتبرعات
        builder.Entity<Case>()
            .HasMany(c => c.Donations)
            .WithOne(d => d.Case)
            .HasForeignKey(d => d.CaseId)
            .OnDelete(DeleteBehavior.Cascade);

        // 6. ربط المستخدمين بالتبرعات
        builder.Entity<Donation>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

        // =================================================================

        // إعدادات الـ Decimal
        builder.Entity<Case>().Property(c => c.RequiredAmount).HasColumnType("decimal(18,2)");
        builder.Entity<Case>().Property(c => c.CollectedAmount).HasColumnType("decimal(18,2)");
        builder.Entity<Donation>().Property(d => d.Amount).HasColumnType("decimal(18,2)");
        builder.Entity<Zakat>().Property(z => z.Amount).HasColumnType("decimal(18,2)");
        builder.Entity<DonorProfile>().Property(p => p.TotalDonated).HasColumnType("decimal(18,2)");
        builder.Entity<DonorProfile>().Property(p => p.TotalDonationsAmount).HasColumnType("decimal(18,2)");
        builder.Entity<Campaign>().Property(c => c.RequiredAmount).HasColumnType("decimal(18,2)");
        builder.Entity<Campaign>().Property(c => c.CollectedAmount).HasColumnType("decimal(18,2)");

        // إدخال الفئات الأساسية (Categories Seed)
        builder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "الصحة", Description = "نعمل على توفير الرعاية الطبية والأدوية اللازمة...", ImageUrl = "/images/categories/الصحه.png" },
            new Category { Id = 2, Name = "التعليم", Description = "نسعى لتوفير فرص تعليمية متكاملة للأطفال...", ImageUrl = "/images/categories/التعليم.png" },
            new Category { Id = 3, Name = "الإغاثة", Description = "نمد يد العون للمتضررين من الكوارث والأزمات...", ImageUrl = "/images/categories/الاغاثة.png" },
            new Category { Id = 4, Name = "كفالات", Description = "اكفل يتيماً أو أسرة محتاجة...", ImageUrl = "/images/categories/كفالات.png" },
            new Category { Id = 5, Name = "مشاريع بناء", Description = "نعمل على بناء وتجديد المساكن...", ImageUrl = "/images/categories/مشاريع بناء.png" },
            new Category { Id = 6, Name = "التنمية", Description = "ندعم مشاريع التنمية المستدامة...", ImageUrl = "/images/categories/التنميه.png" },
            new Category { Id = 7, Name = "ذوى الاحتياجات", Description = "نوفر الرعاية الشاملة والدعم اللازم...", ImageUrl = "/images/categories/ذوى الاحتياجات.png" },
            new Category { Id = 8, Name = "كفارات", Description = "أد كفارتك بيسر وسهولة...", ImageUrl = "/images/categories/كفارات.png" },
            new Category { Id = 9, Name = "الغارمين", Description = "نسعى لمساعدة الغارمين على سداد ديونهم...", ImageUrl = "/images/categories/الغارمين.png" },
            new Category { Id = 10, Name = "الاطعام", Description = "نعمل على توفير وجبات غذائية متكاملة...", ImageUrl = "/images/categories/الاطعام.png" }
        );

        // العلاقات الأخرى
        builder.Entity<Zakat>(entity => {
            entity.HasOne(d => d.Donor).WithMany(u => u.ZakatCalculations).HasForeignKey(d => d.DonorId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(d => d.DonorProfile).WithMany(p => p.Zakats).HasForeignKey(d => d.DonorProfileId).OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<CharityProfile>().HasOne(c => c.User).WithOne(u => u.CharityProfile).HasForeignKey<CharityProfile>(c => c.UserId);
        builder.Entity<DonorProfile>().HasOne(d => d.User).WithOne(u => u.DonorProfile).HasForeignKey<DonorProfile>(d => d.UserId);
        builder.Entity<Report>().HasOne(r => r.Case).WithMany(c => c.Reports).OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Favorite>().HasIndex(f => new { f.UserId, f.CaseId, f.CampaignId }).IsUnique();
    }
}