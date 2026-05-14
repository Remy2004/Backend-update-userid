using Microsoft.AspNetCore.Identity;
using Aoun.DAL.Entities;
using Aoun.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace Aoun.API.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // 1. إنشاء الصلاحيات (Roles)
        string[] roles = { "Admin", "Donor", "Charity" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // 2. إنشاء حساب الأدمن أولاً عشان ناخد الـ Id بتاعه
        var adminEmail = "admin@test.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                Email = adminEmail,
                UserName = adminEmail,
                FirstName = "System",
                UserType = UserType.Admin,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(adminUser, "Admin123!");
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // 3. إنشاء الجمعيات وربطها بـ Id حقيقي (عشان الـ Foreign Key ميضربش)
        //if (!await context.CharityProfiles.AnyAsync())
        //{
        //    context.CharityProfiles.AddRange(
        //        new CharityProfile
        //        {
        //            CharityName = "جمعية الأورمان",
        //            LicenseNumber = "123-ABC",
        //            Description = "جمعية خيرية مصرية تهدف إلى تقديم الدعم والمساعدة للفئات المحتاجة في مختلف المجالات مثل التعليم والصحة والإغاثة.",
        //            Status = ProfileStatus.Approved, // خليناها Approved عشان تظهر
        //            UserId = adminUser.Id // 🔥 استخدمنا الـ Id الحقيقي هنا
        //        },
        //        new CharityProfile
        //        {
        //            CharityName = "مؤسسة مصر الخير",
        //            LicenseNumber = "456-XYZ",
        //            Description = "مؤسسة خيرية مصرية تعمل على تحسين حياة الأفراد والأسر في مصر من خلال برامج تنموية شاملة في مجالات التعليم والصحة والإغاثة.",
        //            Status = ProfileStatus.Approved,
        //            UserId = adminUser.Id // 🔥 وهنا كمان
        //        }
        //    );
        //    await context.SaveChangesAsync();
        //}

        // 4. إنشاء الحالات وربطها بأول جمعية موجودة
        //if (!await context.Cases.AnyAsync())
        //{
        //    var firstCharity = await context.CharityProfiles.FirstOrDefaultAsync();
        //    int charityId = firstCharity?.Id ?? 1;

        //    var sampleCases = new List<Case>
        //    {
        //        new Case {
        //            Title = "إجراء عملية قلب مفتوح لطفل",
        //            Description = "الطفل سيف يحتاج لعملية قلب عاجلة في مركز أسوان للقلب، الحالة حرجة جداً.",
        //            ImageUrl = "/images/default-case.png",
        //            RequiredAmount = 50000, CollectedAmount = 15000,
        //            IsCompleted = false, IsUrgent = false,
        //            CategoryId = 1, CharityId = charityId, CreatedAt = DateTime.UtcNow
        //        },
        //        new Case {
        //            Title = "تجهيز 50 شنطة مدرسية",
        //            Description = "توفير المستلزمات الدراسية للأيتام في قرى صعيد مصر قبل بدء العام الدراسي.",
        //            ImageUrl = "/images/default-case.png",
        //            RequiredAmount = 5000, CollectedAmount = 4800,
        //            IsCompleted = false, IsUrgent = false,
        //            CategoryId = 2, CharityId = charityId, CreatedAt = DateTime.UtcNow
        //        }
        //    };

        //    context.Cases.AddRange(sampleCases);
        //    await context.SaveChangesAsync();
        //}
    }
}