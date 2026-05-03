namespace Aoun.BLL.DTOs.Charity
{
    public class CharityCaseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }

        public decimal RequiredAmount { get; set; }
        public decimal CollectedAmount { get; set; }

        public double Progress { get; set; }

        public bool IsUrgent { get; set; }
        public bool IsCompleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public int DonorsCount { get; set; }
    }
}
