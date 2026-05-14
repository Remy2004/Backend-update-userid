using Aoun.BLL.DTOs.Case;
using Aoun.BLL.Interfaces.Admin;
using Aoun.DAL.Data;
using Aoun.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aoun.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // حماية كاملة: مفيش حد هيدخل هنا غير الأدمن
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(
            IAdminService adminService,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _adminService = adminService;
            _context = context;
            _userManager = userManager;
        }

        // ==========================================
        // 1. الإحصائيات (Dashboard Stats)
        // ==========================================
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _adminService.GetStatsAsync();
            return Ok(new { success = true, data = stats });
        }

        // ==========================================
        // 2. إدارة الجمعيات (Charities Management)
        // ==========================================
        [HttpGet("charities")]
        public async Task<IActionResult> GetAllCharities()
        {
            var charities = await _adminService.GetAllCharitiesAsync();
            return Ok(new { success = true, data = charities });
        }

        [HttpGet("charities/{id:int}")]
        public async Task<IActionResult> GetCharityById(int id)
        {
            var charity = await _adminService.GetCharityByIdAsync(id);
            if (charity == null)
                return NotFound(new { success = false, message = "الجمعية غير موجودة" });

            return Ok(new { success = true, data = charity });
        }

        [HttpPut("charities/{id}/status")]
        public async Task<IActionResult> UpdateCharityStatus(int id, [FromBody] UpdateStatusDto model)
        {
            // هنحول الـ Enum لرقم (1 لـ Approved و 2 لـ Rejected) عشان الـ Service
            int statusInt = (int)model.Status;

            var result = await _adminService.UpdateStatusAsync(id, statusInt);

            if (result)
                return Ok(new { success = true, message = $"تم تحديث حالة الجمعية إلى {model.Status} بنجاح" });

            return BadRequest(new { success = false, message = "فشل التحديث: الجمعية غير موجودة" });
        }

        // ==========================================
        // 3. إدارة الحالات (Cases Management)
        // ==========================================
        [HttpGet("cases")]
        public async Task<IActionResult> GetAllCases()
        {
            var cases = await _adminService.GetAllCasesAsync();
            return Ok(new { success = true, data = cases });
        }

        //[HttpPost("cases")]
        //public async Task<IActionResult> CreateCase([FromBody] CaseCreateDto dto)
        //{
        //   // var newCase = await _adminService.CreateCaseAsync(dto);
        //    return Ok(new { success = true, message = "تم إنشاء الحالة بنجاح", data = newCase });
        //}

        [HttpPut("cases/{id}")]
        public async Task<IActionResult> UpdateCase(int id, [FromBody] CaseUpdateDto dto)
        {
            var updatedCase = await _adminService.UpdateCaseAsync(id, dto);
            return Ok(new { success = true, message = "تم تحديث الحالة بنجاح", data = updatedCase });
        }

        [HttpDelete("cases/{id}")]
        public async Task<IActionResult> DeleteCase(int id)
        {
            var result = await _adminService.DeleteCaseAsync(id);

            if (result)
                return Ok(new { success = true, message = "تم حذف الحالة بنجاح" });

            return BadRequest(new { success = false, message = "لا يمكن حذف الحالة (قد تحتوي على تبرعات أو غير موجودة)" });
        }

        // ==========================================
        // 4. إدارة المستخدمين (Users Management)
        // ==========================================
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    Id = u.Id,
                    Email = u.Email,
                    UserName = u.UserName,
                    Type = u.UserType.ToString()
                })
                .ToListAsync();

            return Ok(new { success = true, data = users });
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { success = false, message = "المستخدم غير موجود" });

            // حماية إضافية: منع الأدمن من حذف نفسه أو حذف System Admin
            if (user.Email == "admin@test.com")
                return BadRequest(new { success = false, message = "لا يمكن حذف حساب الأدمن الأساسي للأنظمة" });

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                return Ok(new { success = true, message = "تم حذف المستخدم بنجاح" });

            return BadRequest(new { success = false, message = "حدث خطأ أثناء الحذف" });
        }

        [HttpPost("add-admin")]
        public async Task<IActionResult> AddAdmin([FromBody] AddAdminDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return BadRequest(new { success = false, message = "هذا البريد الإلكتروني مسجل بالفعل" });

            var newAdmin = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.Email,
                FirstName = dto.FullName,
                UserType = UserType.Admin,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(newAdmin, dto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newAdmin, "Admin");
                return Ok(new { success = true, message = "تمت إضافة مدير جديد للنظام بنجاح" });
            }

            return BadRequest(new { success = false, message = "فشل في إنشاء الحساب، تأكد من قوة كلمة المرور" });
        }

        // ==========================================
        // 5. القوائم المميزة (Top Lists)
        // ==========================================
        [HttpGet("top-donors")]
        public async Task<IActionResult> TopDonors()
        {
            var donors = await _adminService.GetTopDonorsAsync();
            return Ok(new { success = true, data = donors });
        }

        [HttpGet("top-charities")]
        public async Task<IActionResult> TopCharities()
        {
            var charities = await _adminService.GetTopCharitiesAsync();
            return Ok(new { success = true, data = charities });
        }
        [HttpPost("add-charity")]
        public async Task<IActionResult> AddCharityByAdmin([FromBody] AdminAddCharityDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return BadRequest(new { success = false, message = "هذا البريد الإلكتروني مسجل بالفعل" });

            // 1. إنشاء حساب المستخدم للجمعية
            var newCharityUser = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.Email,
                FirstName = dto.CharityName,
                UserType = UserType.Charity,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(newCharityUser, dto.Password);

            if (result.Succeeded)
            {
                // إعطاء صلاحية الجمعية
                await _userManager.AddToRoleAsync(newCharityUser, "Charity");

                // 2. إنشاء بروفايل الجمعية ويكون Approved فوراً لأن الأدمن هو اللي ضافه
                var charityProfile = new CharityProfile
                {
                    UserId = newCharityUser.Id,
                    CharityName = dto.CharityName,
                    LicenseNumber = dto.LicenseNumber,
                    Address = dto.Address ?? "تمت الإضافة بواسطة الإدارة",
                    Status = ProfileStatus.Approved
                };

                _context.CharityProfiles.Add(charityProfile);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "تمت إضافة الجمعية واعتمادها بنجاح" });
            }

            return BadRequest(new { success = false, message = "فشل في إنشاء الحساب، تأكد من قوة كلمة المرور" });
        }
        // ==========================================
        // DTOs الخاصة بالكنترولر
        // ==========================================
        public class UpdateStatusDto
        {
            // Swagger هيجيبلك دروب داون فيها: Pending, Approved, Rejected
            public ProfileStatus Status { get; set; } = ProfileStatus.Approved;
        }

        public class AdminAddCharityDto
        {
            public string CharityName { get; set; } = string.Empty;
            public string LicenseNumber { get; set; } = string.Empty;
            public string? Address { get; set; }
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
        public class AddAdminDto
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
        }
    }
}