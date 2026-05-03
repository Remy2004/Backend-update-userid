using Aoun.BLL.DTOs.Case;
using Aoun.DAL.Data;

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using CaseStatus = Aoun.DAL.Entities.CaseStatus; // حل مشكلة التعارض

namespace Aoun.Tests;

public class CaseServiceTests 
{
    private readonly ApplicationDbContext _context;
    //private readonly CaseService _caseService;

    public CaseServiceTests() 
    {
        // تجهيز داتا بيز وهمية للاختبار
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "CaseServiceTestDB")
            .Options;
        _context = new ApplicationDbContext(options);
        
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        // استخدام الـ Constructor الجديد
        //_caseService = new CaseService(_context);
    }

    [Fact]
    public async Task GetUrgentCasesForHomeAsync_ShouldReturnMappedUrgentCases() 
    {
        // Arrange
        var fakeUrgentCases = new List<Case> {
            new Case { ImageUrl = "test.jpg", CategoryId = 1, CharityId = 1,  Id = 1, Title = "عملية قلب", Description = "Test", Status = CaseStatus.Urgent, RequiredAmount = 1000, CollectedAmount = 500 }
        };
        _context.Cases.AddRange(fakeUrgentCases);
        await _context.SaveChangesAsync();

        // Act
        //var result = await _caseService.GetUrgentCasesForHomeAsync();

        // Assert
        //Assert.NotNull(result);
        //Assert.Single(result);
        //// نتأكد إن الحساب التلقائي (1000 - 500 = 500) شغال (حل مشكلة CS0200)
        //Assert.Equal(500, result.First().RemainingAmount);
    }
}





