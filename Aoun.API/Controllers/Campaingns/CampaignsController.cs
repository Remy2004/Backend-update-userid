//using Aoun.BLL.Dtos;
using Aoun.BLL.DTOs.Campaign;
using Aoun.BLL.Interfaces.Campaign;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aoun.API.Controllers.Campaingns
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignsController : ControllerBase
    {
        private readonly ICampaignService _campaignService;

        public CampaignsController(ICampaignService campaignService)
        {
            _campaignService = campaignService;
        }

        // ================= CREATE =================
        [Authorize(Roles = "Charity")]
        [HttpPost]
        public async Task<ActionResult> CreateCampaign([FromForm] CampaignCreateDto dto)
        {
            var result = await _campaignService.CreateCampaign(dto);
            return Ok(result);
        }

        // ================= HOME =================
        [HttpGet("home")]
        public async Task<ActionResult> GetHomeCampaigns()
        {
            var result = await _campaignService.GetHomeCampaigns();
            return Ok(result);
        }

        // ================= CHARITY =================
        [HttpGet("charity/{charityId}")]
        public async Task<ActionResult> GetCharityCampaigns(
            int charityId,
            string status = "all",
            int page = 1,
            int pageSize = 10)
        {
            var result = await _campaignService.GetCharityCampaigns(
                charityId, status, page, pageSize);

            return Ok(result);
        }

        // ================= PUBLIC =================
        [HttpGet("public")]
        public async Task<ActionResult> GetActiveCampaignsForUsers(
            int page = 1,
            int pageSize = 10)
        {
            var result = await _campaignService.GetActiveCampaignsForUsers(page, pageSize);
            return Ok(result);
        }

        // ================= USER DETAILS =================
        [HttpGet("{id}")]
        public async Task<ActionResult> GetCampaignDetailsForUser(int id)
        {
            var result = await _campaignService.GetCampaignDetailsForUser(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // ================= CHARITY DETAILS =================
        [Authorize(Roles = "Charity")]
        [HttpGet("charity/details/{id}")]
        public async Task<ActionResult> GetCampaignDetailsForCharity(int id)
        {
            var result = await _campaignService.GetCampaignDetailsForCharity(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // ================= UPDATE =================
        [Authorize(Roles = "Charity")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCampaign(int id, [FromForm] UpdateCampaignDto dto)
        {
            var result = await _campaignService.UpdateCampaign(id, dto);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        // ================= DELETE =================
        [Authorize(Roles = "Charity")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCampaign(int id)
        {
            var result = await _campaignService.DeleteCampaign(id);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }
    }
}