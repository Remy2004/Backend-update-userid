namespace Aoun.BLL.DTOs.Case
{
    public class PublicCaseDetailsDto
    {       //ده لليوزر المتبرع لان في واحد تانى للجمعيه لان عندها حاجه زياده ال هى عرض اسماء اخر المتبرعين
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public decimal RequiredAmount { get; set; }
        public decimal CollectedAmount { get; set; }
        public double Progress { get; set; }

        public bool IsUrgent { get; set; }
        public bool IsCompleted { get; set; }

        public string CategoryName { get; set; }
        public string CharityName { get; set; }

        public int DonorsCount { get; set; }

    }
}