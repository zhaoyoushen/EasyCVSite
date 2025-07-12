using System.ComponentModel.DataAnnotations;

namespace PersonalHomepage.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class DashboardViewModel
    {
        public PageConfiguration UserProfile { get; set; } = new PageConfiguration();
        public string? PublicUrl { get; set; }
        public bool IsPublished { get; set; }
        public int TotalViews { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public string UserDisplayName { get; set; } = string.Empty;
    }

    public class SetCustomUrlRequest
    {
        public string CustomUrl { get; set; } = string.Empty;
    }
}