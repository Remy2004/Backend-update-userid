using Aoun.DAL.Entities;
using Aoun.DAL.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aoun.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous] // Home data should be public
public class HomeController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    public HomeController(IUnitOfWork uow) => _uow = uow;

    [HttpGet]
    public async Task<IActionResult> GetHomeData()
    {
        var cases = await _uow.Repository<Case>().GetAllAsync();
        var charities = await _uow.Repository<CharityProfile>().GetAllAsync();

        return Ok(new
        {
            Categories = new[] { "Orphans", "Health", "Relief", "Education" },
            TrendingCases = cases.Where(c => !c.IsDeleted)
                                .OrderByDescending(c => c.Status == CaseStatus.Urgent)
                                .Take(5),
            FeaturedCharities = charities.Where(c => !c.IsDeleted && c.Status == ProfileStatus.Approved)
                                        .Take(3)
        });
    }
}

