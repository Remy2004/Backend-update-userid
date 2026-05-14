using Aoun.BLL.DTOs.Case;
using Aoun.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace Aoun.API.Controllers.Cases
{
    [ApiController]
    [Route("api/[controller]")]
    public class CasesController : ControllerBase
    {
        private readonly ICaseService _service;

        public CasesController(ICaseService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string? categoryName, string status, int page = 1, int pageSize = 10)
        {
            var result = await _service.GetAllCases(categoryName, status, page, pageSize);
            return Ok(new
            {
                result.Data,
                result.TotalCount,
                page,
                pageSize
            });
        }

        [HttpGet("home")]
        public async Task<IActionResult> Home()
        {
            return Ok(await _service.GetHomeCases());
        }

        [Authorize(Roles = "Charity")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CaseCreateDto dto)

        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

           // var result = await _campaignService.CreateCampaign(dto, userId);
            return Ok(await _service.CreateCase(dto,userId));
        }



        //[Authorize(Roles = "Charity")]
        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(int id, [FromForm] CaseUpdateDto dto)
        //{
        //    var result = await _service.UpdateCase(id, dto);

        //    if (!result.Success)
        //        return BadRequest(new
        //        {
        //            result.Success,
        //            result.Message
        //        });

        //    return Ok(new
        //    {
        //        success = result.Success,
        //        message = result.Message,
        //        data = result.Data
        //    });
        //}

        [Authorize(Roles = "Charity")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] CaseUpdateDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _service.UpdateCase(id, dto, userId);

            if (!result.Success)
                return BadRequest(new
                {
                    result.Success,
                    result.Message
                });

            return Ok(new
            {
                success = result.Success,
                message = result.Message,
                data = result.Data
            });
        }





        //[Authorize(Roles = "Charity")]
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var result = await _service.DeleteCase(id);

        //    if (!result.Success)
        //        return Ok(new
        //        {
        //            success = false,
        //            message = result.Message
        //        });

        //    return Ok(new
        //    {
        //        success = result.Success,
        //        message = result.Message,
        //        data = result.DeletedCase
        //    });
        //}


        [Authorize(Roles = "Charity")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _service.DeleteCase(id, userId);

            if (!result.Success)
                return BadRequest(new
                {
                    success = false,
                    message = result.Message
                });

            return Ok(new
            {
                success = result.Success,
                message = result.Message,
                data = result.DeletedCase
            });
        }




        [HttpGet("public/{id}")]
        public async Task<IActionResult> Public(int id)
        {
            var result = await _service.GetPublicCaseDetails(id);
            if (result == null) return NotFound();
            return Ok(result);
        }



        [HttpGet("search")]
        public async Task<IActionResult> Search(
        int? categoryId,
        string status = "all",
        string? keyword = null,
        string? charityName = null,
        int page = 1,
        int pageSize = 10)
        {
            var result = await _service.SearchCases(categoryId, status, keyword, charityName, page, pageSize);

            return Ok(new
            {
                data = result.Data,
                totalCount = result.TotalCount
            });

        }

        //[Authorize(Roles = "Charity")]
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetCaseDetails(int id)
        //{
        //    var result = await _service.GetCaseDetails(id);

        //    if (result == null)
        //        return NotFound(new { message = "Case not found" });

        //    return Ok(new { data = result });
        //}


        [Authorize(Roles = "Charity")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCaseDetails(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _service.GetCaseDetails(id, userId);

            if (result == null)
                return NotFound(new { message = "غير مصرح أو الحالة غير موجودة" });

            return Ok(new { data = result });
        }




        [HttpGet("charity/{charityId}/cases")]
        public async Task<IActionResult> GetCharityCasesByFilter(
            int charityId,
            string status = "all",
            int? categoryId = null,
           int page = 1,
            int pageSize = 10)
        {
            var result = await _service.GetCharityCasesByFilter(charityId, status, categoryId, page, pageSize);

            return Ok(new
            {
                stats = result.Stats,
                cases = result.Cases,
                totalCount = result.TotalCount,
                page,
                pageSize
            });
        }

    }

}



