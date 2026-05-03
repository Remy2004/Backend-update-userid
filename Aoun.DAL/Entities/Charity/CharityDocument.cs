using CharityEntity = Aoun.DAL.Entities.Charity.Charity;
namespace Aoun.DAL.Entities
{
    public class CharityDocument
    {
        public int Id { get; set; }

        public int CharityProfileId { get; set; }
        public string DocumentName { get; set; } = string.Empty; // تأكد من إضافتها
        public string DocumentUrl { get; set; } = string.Empty;

        public int CharityId { get; set; }

        public string DocumentType { get; set; } = string.Empty;

        public string FileUrl { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; }

        public CharityEntity Charity { get; set; } = null!;
    }
}