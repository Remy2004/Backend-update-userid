using Microsoft.AspNetCore.Http;

namespace Aoun.BLL.DTOs.Case
{
    public class CaseUpdateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
     // public string? ImageUrl { get; set; } // جديد
        public decimal RequiredAmount { get; set; }
        public bool IsUrgent { get; set; }
        public int CategoryId { get; set; }  // جديد
    }
}
