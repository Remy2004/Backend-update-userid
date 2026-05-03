namespace Aoun.BLL.DTOs.Donations
{
    public class DonationListItemDto
    {
        public int Id { get; set; }
        public string DonorName { get; set; }
        public decimal Amount { get; set; }
        public bool IsGift { get; set; }
        public DateTime Date { get; set; }
    }
}
