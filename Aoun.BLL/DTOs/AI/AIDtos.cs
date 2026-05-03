using System.ComponentModel.DataAnnotations;

namespace Aoun.BLL.DTOs.AI
{
    public class GenerateDescDto
    {
        [Required] public string Title { get; set; } = string.Empty;
        [Required] public string Category { get; set; } = string.Empty;
        [Required] public decimal RequiredAmount { get; set; }
    }

    public class AIChatRequestDto
    {
        [Required] public string Message { get; set; } = string.Empty;
    }
}