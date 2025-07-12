using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PersonalHomepage.Models;

namespace PersonalHomepage.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<UserProfile> UserProfiles { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // 配置UserProfile
            builder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.ConfigurationJson).HasColumnType("nvarchar(max)");
                
                // 配置与ApplicationUser的关系
                entity.HasOne(e => e.User)
                      .WithOne(u => u.Profile)
                      .HasForeignKey<UserProfile>(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                      
                // 创建索引
                entity.HasIndex(e => e.UserId).IsUnique();
            });
            
            // 配置ApplicationUser
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.HasIndex(e => e.CustomUrl).IsUnique();
                entity.Property(e => e.CustomUrl).HasMaxLength(50);
                entity.Property(e => e.DisplayName).HasMaxLength(100);
            });
        }
    }
}