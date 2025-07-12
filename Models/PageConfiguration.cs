using System.ComponentModel.DataAnnotations;

namespace PersonalHomepage.Models
{
    public class PageConfiguration
    {
        public PersonalInfo? PersonalInfo { get; set; }
        public List<Skill>? Skills { get; set; }
        public List<Project>? Projects { get; set; }
        public ContactInfo? ContactInfo { get; set; }
        public ThemeSettings? Theme { get; set; }
    }

    public class PersonalInfo
    {
        public string? Name { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? WelcomeMessage { get; set; }
        public List<string>? Highlights { get; set; }
    }

    public class Project
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? DetailedDescription { get; set; }
        public List<string>? Technologies { get; set; }
        public string? DemoUrl { get; set; }
        public string? SourceCodeUrl { get; set; }
        public DateTime? CompletedDate { get; set; }
    }

    public class ContactInfo
    {
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Location { get; set; }
        public List<SocialLink>? SocialLinks { get; set; }
    }

    public class SocialLink
    {
        public string? Platform { get; set; }
        public string? Url { get; set; }
        public string? Icon { get; set; }
    }

    public class ThemeSettings
    {
        public string? PrimaryColor { get; set; }
        public string? SecondaryColor { get; set; }
        public string? BackgroundColor { get; set; }
        public string? TextColor { get; set; }
        public string? FontFamily { get; set; }
        public DisplayStyle DisplayStyle { get; set; } = DisplayStyle.Modern;
    }

    public enum DisplayStyle
    {
        Modern = 0,      // Modern Style (Default)
        Classic = 1,     // Classic Style
        Minimal = 2,     // Minimal Style
        Creative = 3,    // Creative Style
        Professional = 4 // Professional Style
    }

    public class ConfigurationUpload
    {
        [Required]
        public IFormFile? ConfigFile { get; set; }
    }
}