namespace Aoun.DAL.Entities;
public class TrustScore {
    public int Id { get; set; }
    public int Score { get; set; }
    public bool IsDeleted { get; set; } = false;
    public int CharityProfileId { get; set; }
    public CharityProfile? CharityProfile { get; set; }
}







