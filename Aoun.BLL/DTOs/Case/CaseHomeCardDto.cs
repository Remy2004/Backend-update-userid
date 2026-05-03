namespace Aoun.BLL.DTOs.Case
{
    public class CaseHomeCardDto
    {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string ImageUrl { get; set; }
            public decimal RequiredAmount { get; set; }
            public decimal CollectedAmount { get; set; }
            public bool IsUrgent { get; set; }
        
    }
}
