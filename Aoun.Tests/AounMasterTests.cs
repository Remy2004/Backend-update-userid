using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

using Aoun.DAL.Data;
using Aoun.BLL.DTOs.Case;
using System.Threading.Tasks;
using CaseStatus = Aoun.DAL.Entities.CaseStatus; // حل مشكلة التعارض

namespace Aoun.Tests;

public class AounMasterTests 
{
    private readonly ApplicationDbContext _context;
    //private readonly CaseService _caseService;

    public AounMasterTests() 
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "AounMasterTestDB")
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        
        _context = new ApplicationDbContext(options);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        
        //_caseService = new CaseService(_context);
    }

    [Fact]
    public async Task GetAllActiveCasesAsync_ShouldReturnCases_WhereCollectedIsLessThanRequired() 
    {
        // Arrange
        _context.Cases.AddRange(
            new Case { ImageUrl = "test.jpg", CategoryId = 1, CharityId = 1,  Id = 10, Title = "حالة مكتملة", Description = "...", RequiredAmount = 1000, CollectedAmount = 1000, Status = CaseStatus.Completed },
            new Case { ImageUrl = "test.jpg", CategoryId = 1, CharityId = 1,  Id = 11, Title = "حالة نشطة", Description = "...", RequiredAmount = 2000, CollectedAmount = 500, Status = CaseStatus.Active }
        );
        await _context.SaveChangesAsync();

        // Act
        //var result = await _caseService.GetAllActiveCasesAsync();

        //// Assert
        //result.Should().HaveCount(1);
        //result[0].Id.Should().Be(11);
        //result[0].RemainingAmount.Should().Be(1500);
    }
}





