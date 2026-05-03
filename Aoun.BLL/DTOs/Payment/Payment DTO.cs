namespace Aoun.BLL.DTOs.Payment
{
    public class PaymentDto
    {
   
       
        public int DonationId { get; set; }
        public string PaymentMethod { get; set; } // ضيفي — "Card" / "GooglePay" / "Wallet"
        public string? CardNumber { get; set; }
        public string? ExpiryDate { get; set; }
        public string? CVV { get; set; }
        public string? CardHolderName { get; set; }

    }
}
