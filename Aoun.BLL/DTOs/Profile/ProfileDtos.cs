namespace Aoun.BLL.DTOs.Profile;

public class UpdateProfileDto
{
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string PhoneNumber { get; set; } = "";

}

public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = "";
    public string NewPassword { get; set; } = "";
}
