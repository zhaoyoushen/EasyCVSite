namespace PersonalHomepage.Models
{
    public class WorkExperience
    {
        public int Id { get; set; }
        public string? Company { get; set; }
        public string? Position { get; set; }
        public string? Description { get; set; }
        public string? DetailedDescription { get; set; }
        public List<string>? Technologies { get; set; }
        public List<string>? Responsibilities { get; set; }
        public List<string>? Achievements { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
        public bool IsCurrent { get; set; }
        public string? CompanyWebsite { get; set; }
        public string? CompanyLogo { get; set; }
    }
}