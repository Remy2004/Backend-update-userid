using System;

namespace Aoun.BLL.DTOs
{
    public class CaseGetAllDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal RequiredAmount { get; set; }
        public decimal CollectedAmount { get; set; }
        public bool IsUrgent { get; set; }
        public bool IsCompleted { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
