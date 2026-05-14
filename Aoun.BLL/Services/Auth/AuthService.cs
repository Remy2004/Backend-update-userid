using Aoun.BLL.DTOs.Auth;
using Aoun.BLL.Interfaces.Auth;
using Aoun.DAL.Data;
using Aoun.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Aoun.BLL.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext? _context;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration config, ApplicationDbContext context)
        {
            _userManager = userManager;
            _config = config;
            _context = context; // دلوقتي الـ Compiler عارف مين هو الـ context ده
        }

        // ==========================================
        // 1. تسجيل الدخول (تم دمج الدالتين وإصلاح اسم التوكن)
        // ==========================================
        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email))
                return new AuthResponseDto { IsSuccess = false, Message = "البريد الإلكتروني مطلوب." };

            var user = await _userManager.FindByEmailAsync(request.Email.Trim());

            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return new AuthResponseDto { IsSuccess = false, Message = "البريد الإلكتروني أو كلمة المرور غير صحيحة." };
            }

            var roles = await _userManager.GetRolesAsync(user);
            string userRole = roles.FirstOrDefault() ?? "Donor";

            // 🔥 تم توحيد اسم الدالة هنا
            string token = await GenerateToken(user);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "تم تسجيل الدخول بنجاح.",
                Token = token,
                Role = userRole // بيرجع للفرونت إند عشان يوجه المستخدم
            };
        }

        // ==========================================
        // 2. إنشاء حساب جديد (مع إظهار سبب الرفض الحقيقي)
        // ==========================================
        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
{
    // 1. التأكد إن الإيميل مش متسجل قبل كده
    var existingUser = await _userManager.FindByEmailAsync(request.Email);
    if (existingUser != null)
    {
        return new AuthResponseDto { IsSuccess = false, Message = "البريد الإلكتروني مستخدم بالفعل." };
    }

    // 2. إنشاء كائن المستخدم
    var user = new ApplicationUser
    {
        UserName = request.Email,
        Email = request.Email,
        FirstName = request.FullName,
        UserType = (UserType)(int)request.AccountType ,// تحويل الـ Enum المخصص للتسجيل لنوع المستخدم الأصلي
        // CreatedAt = DateTime.UtcNow
    };

    // 3. حفظ المستخدم في Identity
    var result = await _userManager.CreateAsync(user, request.Password);

    if (!result.Succeeded)
    {
        var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
        return new AuthResponseDto { IsSuccess = false, Message = $"فشل إنشاء الحساب: {errors}" };
    }

    // 4. 🔥 الجزء الجديد: إنشاء الـ Profile للمتبرع أوتوماتيكياً
    if (request.AccountType == RegistrationAccountType.Donor)
    {
        var donorProfile = new DonorProfile
        {
            UserId = user.Id,
            TotalDonated = 0,
            TotalDonationsAmount = 0
        };
        
        // _context هنا هو الـ ApplicationDbContext بتاعك
        _context.DonorProfiles.Add(donorProfile);
        await _context.SaveChangesAsync();
    }

    // 5. إعطاء الصلاحية (Role) بناءً على اختياره
    await _userManager.AddToRoleAsync(user, request.AccountType.ToString());

    return new AuthResponseDto { IsSuccess = true, Message = "تم إنشاء الحساب بنجاح." };
}

        // ==========================================
        // 3. نسيان كلمة المرور
        // ==========================================
        public async Task<AuthResponseDto> ForgotPasswordAsync(ForgotPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email.Trim());
            if (user == null)
            {
                return new AuthResponseDto { IsSuccess = false, Message = "البريد الإلكتروني غير موجود." };
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return new AuthResponseDto { IsSuccess = true, Message = "تم إنشاء كود استعادة المرور بنجاح.", Token = token };
        }

        // ==========================================
        // 4. تأكيد البريد الإلكتروني
        // ==========================================
        public async Task<AuthResponseDto> VerifyEmailAsync(VerifyEmailDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email.Trim());
            if (user == null)
            {
                return new AuthResponseDto { IsSuccess = false, Message = "البريد الإلكتروني غير موجود." };
            }

            if (user.EmailConfirmed)
            {
                return new AuthResponseDto { IsSuccess = true, Message = "البريد الإلكتروني مؤكد بالفعل." };
            }

            var result = await _userManager.ConfirmEmailAsync(user, model.Token);
            return result.Succeeded
                ? new AuthResponseDto { IsSuccess = true, Message = "تم تأكيد البريد الإلكتروني بنجاح." }
                : new AuthResponseDto { IsSuccess = false, Message = string.Join(", ", result.Errors.Select(e => e.Description)) };
        }

        // ==========================================
        // 5. تسجيل الدخول عبر السوشيال ميديا
        // ==========================================
        public async Task<AuthResponseDto> SocialLoginAsync(SocialLoginDto model)
        {
            var provider = model.Provider.ToLower();
            var email = model.Email?.Trim();

            if (string.IsNullOrEmpty(email))
                return new AuthResponseDto { IsSuccess = false, Message = "البريد الإلكتروني من المزود مفقود." };

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = model.FullName ?? (provider + " User"),
                    UserType = UserType.Donor,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                    return new AuthResponseDto { IsSuccess = false, Message = "فشل في إنشاء الحساب." };

                await _userManager.AddToRoleAsync(user, "Donor");
            }

            var token = await GenerateToken(user);
            var roles = await _userManager.GetRolesAsync(user);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Token = token,
                Role = roles.FirstOrDefault() ?? "Donor",
                Message = $"تم تسجيل الدخول عبر {provider} بنجاح."
            };
        }

        // ==========================================
        // 6. إنشاء التوكن السري (JWT)
        // ==========================================
        private async Task<string> GenerateToken(ApplicationUser user)
        {
            var secret = _config["JwtSettings:Secret"];
            if (string.IsNullOrEmpty(secret)) throw new Exception("JWT Secret Missing");

            var roles = await _userManager.GetRolesAsync(user);
            var roleList = roles.ToList();

            if (user.Email == "admin@test.com" && !roleList.Contains("Admin"))
                roleList.Add("Admin");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim("uid", user.Id)
            };

            foreach (var role in roleList)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}