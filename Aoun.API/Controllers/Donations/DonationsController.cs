using Aoun.BLL.DTOs.Donations;
using Aoun.BLL.DTOs.Payment;
using Aoun.BLL.Interfaces.Donation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Aoun.API.Controllers.Donations
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonationsController : ControllerBase
    {
        private readonly IDonationService _service;

        public DonationsController(IDonationService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDonation(DonationCreateDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _service.CreateDonation(dto,userId);

            return Ok(result);
        }

        [HttpPost("pay")]
        public async Task<IActionResult> Pay(PaymentDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _service.Pay(dto, userId);

            return Ok(result);
        }

        //[HttpGet("/api/cases/{caseId}/donations")]
        //public async Task<IActionResult> GetCaseDonations(int caseId, int page = 1, int pageSize = 10)
        //{
        //    var result = await _service.GetCaseDonations(caseId, page, pageSize);
        //    return Ok(result);
        //}

        [Authorize(Roles = "Charity,Admin")]
        [HttpGet("/api/cases/{caseId}/donations")]
        public async Task<IActionResult> GetCaseDonations(int caseId, int page = 1, int pageSize = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _service.GetCaseDonations(caseId, page, pageSize, userId);

            return Ok(result);
        }



        //[HttpGet("/api/campaigns/{campaignId}/donations")]
        //public async Task<IActionResult> GetCampaignDonations(int campaignId, int page = 1, int pageSize = 10)
        //{
        //    var result = await _service.GetCampaignDonations(campaignId, page, pageSize);
        //    return Ok(result);
        //}

        [Authorize(Roles = "Charity,Admin")]
        [HttpGet("/api/campaigns/{campaignId}/donations")]
        public async Task<IActionResult> GetCampaignDonations(int campaignId, int page = 1, int pageSize = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _service.GetCampaignDonations(campaignId, page, pageSize, userId);

            if (result == null)
                return NotFound(new { message = "غير مصرح أو الحملة غير موجودة" });

            return Ok(result);
        }

    }
}