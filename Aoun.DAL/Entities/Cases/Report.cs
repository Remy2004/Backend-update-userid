namespace Aoun.DAL.Entities.Cases
{
    public class Report
    {
        public int Id { get; set; }
        public int CaseId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Navigation
        public global::Aoun.DAL.Entities.Case Case { get; set; }
    }
}
