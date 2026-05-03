namespace Aoun.BLL.DTOs.Auth
{
    public enum RegistrationAccountType
    {
        Donor = 2,
        Charity = 3
    }

    public class RegisterRequestDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // 🔥 نستخدم الـ Enum الجديد هنا عشان Swagger يعرض (Donor, Charity) بس
        public RegistrationAccountType AccountType { get; set; } = RegistrationAccountType.Donor;
    }

    public class RegisterDto : RegisterRequestDto { }

    public class LoginRequestDto
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class AuthResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = "";
        public string Token { get; set; } = "";
        public string Role { get; set; } = "";
    }

    public class ForgotPasswordDto
    {
        public string Email { get; set; } = "";
    }

    public class VerifyEmailDto
    {
        public string Email { get; set; } = "";
        public string Token { get; set; } = "";
    }

    // 🔥 خلينا النسخة الأشمل ومسحنا المكررة
    public class SocialLoginDto
    {
        public string Provider { get; set; } = string.Empty; // google or facebook
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string Token { get; set; } = string.Empty;
    }

    public class CharityRegistrationDto
    {
        public string CharityName { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public string? Address { get; set; }

        // 🔥 ضفنا رابط المستند هنا عشان يتبعتوا كلهم مرة واحدة
        public string DocumentUrl { get; set; } = string.Empty;
    }
}