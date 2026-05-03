using Aoun.BLL.DTOs.AI;
using Aoun.BLL.Services.Chat;
using Aoun.DAL.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;

namespace Aoun.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIController : ControllerBase
    {
        private readonly AISmartService _aiService;
        private readonly ApplicationDbContext _context;

        public AIController(AISmartService aiService, ApplicationDbContext context)
        {
            _aiService = aiService;
            _context = context;
        }

        // ==========================================
        // 1. مولد الوصف الذكي (مفتوح للأدمن أو الجمعيات)
        // ==========================================
        [HttpPost("generate-description")]
        public async Task<IActionResult> GenerateDescription([FromBody] GenerateDescDto request)
        {
            try
            {
                var description = await _aiService.GenerateCaseDescriptionAsync(request.Title, request.Category, request.RequiredAmount);
                return Ok(new { success = true, generatedText = description });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "AI Error: " + ex.Message });
            }
        }

        // ==========================================
        // 2. البوت الذكي (RAG Chatbot)
        // ==========================================
        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] AIChatRequestDto request)
        {
            try
            {
                // RAG Step 1: سحب أحدث 5 حالات عاجلة من الداتا بيز لإعطائها للـ AI كمرجع
                var activeCases = await _context.Cases
                    .Include(c => c.Category)
                    .Where(c => c.IsUrgent && c.CollectedAmount < c.RequiredAmount)
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(5)
                    .Select(c => $"- حالة '{c.Title}' (قسم {c.Category.Name}): مطلوب {c.RequiredAmount} تم جمع {c.CollectedAmount}")
                    .ToListAsync();

                var dbContextString = activeCases.Any()
                    ? "الحالات العاجلة الحالية:\n" + string.Join("\n", activeCases)
                    : "لا توجد حالات عاجلة حالياً.";

                // RAG Step 2: إرسال السياق (Context) + سؤال المستخدم لـ Gemini
                var answer = await _aiService.ChatWithRAGAsync(request.Message, dbContextString);
                return Ok(new { success = true, answer = answer });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "AI Chat Error: " + ex.Message });
            }
        }

        // ==========================================
        // 3. نظام التوصيات المخصص (يحتاج تسجيل دخول)
        // ==========================================
        // ==========================================
        // 3. نظام التوصيات المخصص (يحتاج تسجيل دخول)
        // ==========================================
        [Authorize]
        [HttpGet("recommendations")]
        public async Task<IActionResult> GetRecommendations()
        {
            // 1. حل مشكلة تداخل الأسماء باستخدام اسم متغير مختلف (currentUserId)
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

            try
            {
                // 2. حل مشكلة الـ TargetType ومطابقة الـ ID بأمان
                // قمنا باستخدام ToString() لضمان مطابقة الـ ID سواء كان في الداتا بيز int أو String
                // واستخدمنا CaseId بدلاً من TargetType لنجلب (اسم القسم) الذي تبرع له المستخدم
                var userCategories = await _context.Donations
                    .Where(d => d.UserId.ToString() == currentUserId && d.CaseId != null)
                    .Select(d => d.Case.Category.Name)
                    .Distinct()
                    .ToListAsync();

                // 3. بناء الجملة التي سيفهمها Gemini
                var historyString = userCategories.Any()
                    ? "المستخدم يحب التبرع في الأقسام التالية: " + string.Join(" و ", userCategories)
                    : "هذا مستخدم جديد ليس لديه تاريخ تبرعات سابق. اقترح عليه أقوى حالات المنصة وأكثرها أهمية.";

                // 4. جلب الحالات المتاحة
                var availableCases = await _context.Cases
                    .Where(c => c.CollectedAmount < c.RequiredAmount)
                    .Take(10)
                    .Select(c => $"- {c.Title} (مطلوب: {c.RequiredAmount})")
                    .ToListAsync();

                var casesString = availableCases.Any() ? string.Join("\n", availableCases) : "لا يوجد حالات متاحة حالياً.";

                // 5. استدعاء Gemini ليعطي النصيحة
                var recommendation = await _aiService.GetRecommendationsAsync(historyString, casesString);

                return Ok(new { success = true, recommendation = recommendation });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "AI Recommendation Error: " + ex.Message });
            }
        }
    }
}
