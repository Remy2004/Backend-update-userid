using Aoun.BLL.DTOs.Auth;
using Aoun.BLL.DTOs.Document;
using Aoun.BLL.Interfaces; // أو Aoun.BLL.Interfaces.Auth حسب مكان الإنترفيس عندك
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Aoun.API.Controllers.Charity
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Charity")] // 🔥 حماية: ده مخصص لحسابات الجمعيات فقط
    public class CharityController : ControllerBase
    {
        private readonly ICharityService _charityService;

        public CharityController(ICharityService charityService)
        {
            _charityService = charityService;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

        // ==========================================
        // 1. تقديم الطلب للإدارة 
        // ==========================================
        [HttpPost("complete-profile")]
        public async Task<IActionResult> Complete([FromBody] CharityRegistrationDto model)
        {
            var result = await _charityService.CompleteProfileAsync(model, GetUserId());

            if (!result)
                return BadRequest(new { success = false, message = "حدث خطأ أثناء حفظ البيانات" });

            return Ok(new
            {
                success = true,
                message = "تم حفظ البيانات بنجاح، برجاء رفع المستندات"
            });
        }


        // ==========================================
        // 2. استعلام الجمعية عن حالة طلبها الحالي
        // ==========================================
        [HttpGet("status")]
        public async Task<IActionResult> GetMyStatus()
        {
            var status = await _charityService.GetCharityStatusAsync(GetUserId());

            if (status == null)
                return NotFound(new { success = false, message = "لم يتم تقديم أي طلبات تسجيل حتى الآن." });

            return Ok(new { success = true, data = status });
        }



        // ==========================================
        // 2. Upload Documents (UNCHANGED)
        // ==========================================
        [Authorize(Roles = "Charity")]
        [HttpPost("upload-documents")]
        public async Task<IActionResult> UploadDocuments([FromForm] CharityDocumentsDto dto)
        {
            var result = await _charityService.UploadDocumentsAsync(dto, GetUserId());

            if (!result)
                return BadRequest(new { success = false, message = "الجمعية غير موجودة" });

            return Ok(new
            {
                success = true,
                message = "تم تقديم الطلب بنجاح وسيتم مراجعته من الإدارة"
            });
        }



    }
}