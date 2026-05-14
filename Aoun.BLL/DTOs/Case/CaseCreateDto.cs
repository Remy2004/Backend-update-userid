using Microsoft.AspNetCore.Http;

namespace Aoun.BLL.DTOs.Case
{
    public class CaseCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
        public decimal RequiredAmount { get; set; }
        public bool IsUrgent { get; set; }
        public int CategoryId { get; set; }
      
    }
}
