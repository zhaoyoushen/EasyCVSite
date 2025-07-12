using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PersonalHomepage.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100)]
        public string? DisplayName { get; set; }
        
        [MaxLength(50)]
        public string? CustomUrl { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? LastLoginAt { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        // 导航属性
        public virtual UserProfile? Profile { get; set; }
    }
    
    public class UserProfile
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        public string? ConfigurationJson { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsPublished { get; set; } = false;
        
        public int ViewCount { get; set; } = 0;
        
        // 导航属性
        public virtual ApplicationUser User { get; set; } = null!;
    }
}