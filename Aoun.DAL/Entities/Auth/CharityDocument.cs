namespace Aoun.DAL.Entities.Auth;
public class CharityDocument {
    public int Id { get; set; }
    public string DocumentUrl { get; set; } = string.Empty;
    public string DocumentName { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public int CharityProfileId { get; set; }
    public CharityProfile? CharityProfile { get; set; }
}













