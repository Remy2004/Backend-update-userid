namespace Aoun.BLL.DTOs.Case
{
    public class CaseUpdatedResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsUrgent { get; set; }
        public decimal RequiredAmount { get; set; }
    }
}
