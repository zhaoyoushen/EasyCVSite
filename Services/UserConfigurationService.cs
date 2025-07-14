using Microsoft.EntityFrameworkCore;
using PersonalHomepage.Data;
using PersonalHomepage.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PersonalHomepage.Services
{
    public interface IUserConfigurationService
    {
        Task<PageConfiguration> GetUserConfigurationAsync(string userId);
        Task<PageConfiguration?> GetPublicConfigurationAsync(string customUrl);
        Task SaveUserConfigurationAsync(string userId, PageConfiguration configuration);
        Task<bool> UploadConfigurationAsync(string userId, IFormFile file);
        Task<bool> PublishConfigurationAsync(string userId);
        Task<bool> UnpublishConfigurationAsync(string userId);
        Task<bool> SetCustomUrlAsync(string userId, string customUrl);
        Task<bool> IsCustomUrlAvailableAsync(string customUrl, string? excludeUserId = null);
        Task<UserProfile?> GetUserProfileAsync(string userId);
        Task IncrementViewCountAsync(string customUrl);
    }

    public class UserConfigurationService : IUserConfigurationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserConfigurationService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public UserConfigurationService(
            ApplicationDbContext context,
            ILogger<UserConfigurationService> logger)
        {
            _context = context;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        public async Task<PageConfiguration> GetUserConfigurationAsync(string userId)
        {
            try
            {
                var profile = await _context.UserProfiles
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                if (profile?.ConfigurationJson != null)
                {
                    var configuration = JsonSerializer.Deserialize<PageConfiguration>(
                        profile.ConfigurationJson, _jsonOptions);
                    return configuration ?? GetDefaultConfiguration();
                }

                return GetDefaultConfiguration();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user configuration for user {UserId}", userId);
                return GetDefaultConfiguration();
            }
        }

        public async Task<PageConfiguration?> GetPublicConfigurationAsync(string customUrl)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Profile)
                    .FirstOrDefaultAsync(u => u.CustomUrl == customUrl && u.IsActive);

                if (user?.Profile?.IsPublished == true && user.Profile.ConfigurationJson != null)
                {
                    var configuration = JsonSerializer.Deserialize<PageConfiguration>(
                        user.Profile.ConfigurationJson, _jsonOptions);
                    return configuration;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting public configuration for URL {CustomUrl}", customUrl);
                return null;
            }
        }

        public async Task SaveUserConfigurationAsync(string userId, PageConfiguration configuration)
        {
            try
            {
                var profile = await _context.UserProfiles
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                var json = JsonSerializer.Serialize(configuration, _jsonOptions);

                if (profile == null)
                {
                    profile = new UserProfile
                    {
                        UserId = userId,
                        ConfigurationJson = json,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.UserProfiles.Add(profile);
                }
                else
                {
                    profile.ConfigurationJson = json;
                    profile.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Configuration saved for user {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving configuration for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> UploadConfigurationAsync(string userId, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return false;

                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();

                // 验证JSON格式
                var configuration = JsonSerializer.Deserialize<PageConfiguration>(json, _jsonOptions);
                if (configuration == null)
                    return false;

                // 保存用户配置
                await SaveUserConfigurationAsync(userId, configuration);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading configuration file for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> PublishConfigurationAsync(string userId)
        {
            try
            {
                var profile = await _context.UserProfiles
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                if (profile != null)
                {
                    profile.IsPublished = true;
                    profile.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing configuration for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> UnpublishConfigurationAsync(string userId)
        {
            try
            {
                var profile = await _context.UserProfiles
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                if (profile != null)
                {
                    profile.IsPublished = false;
                    profile.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unpublishing configuration for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> SetCustomUrlAsync(string userId, string customUrl)
        {
            try
            {
                // 检查URL是否可用
                if (!await IsCustomUrlAvailableAsync(customUrl, userId))
                {
                    return false;
                }

                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.CustomUrl = customUrl;
                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting custom URL for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> IsCustomUrlAvailableAsync(string customUrl, string? excludeUserId = null)
        {
            try
            {
                var query = _context.Users.Where(u => u.CustomUrl == customUrl);
                
                if (!string.IsNullOrEmpty(excludeUserId))
                {
                    query = query.Where(u => u.Id != excludeUserId);
                }

                return !await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking custom URL availability");
                return false;
            }
        }

        public async Task<UserProfile?> GetUserProfileAsync(string userId)
        {
            try
            {
                return await _context.UserProfiles
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile for user {UserId}", userId);
                return null;
            }
        }

        public async Task IncrementViewCountAsync(string customUrl)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Profile)
                    .FirstOrDefaultAsync(u => u.CustomUrl == customUrl);

                if (user?.Profile != null)
                {
                    user.Profile.ViewCount++;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing view count for URL {CustomUrl}", customUrl);
            }
        }

        private PageConfiguration GetDefaultConfiguration()
        {
            return new PageConfiguration
            {
                PersonalInfo = new PersonalInfo
                {
                    Name = "Your Name",
                    Title = "Professional Title",
                    Description = "Tell the world about yourself and your expertise.",
                    ProfileImageUrl = "https://via.placeholder.com/400",
                    WelcomeMessage = "Welcome to My Personal Homepage",
                    Highlights = new List<string>
                    {
                        "Add your professional highlights here."
                    }
                },
                Skills = new List<Skill>(),
                Projects = new List<Project>(),
                ContactInfo = new ContactInfo
                {
                    Email = "your.email@example.com",
                    Phone = "+1 (555) 123-4567",
                    Location = "Your City, Country",
                    SocialLinks = new List<SocialLink>()
                },
                Theme = new ThemeSettings
                {
                    DisplayStyle = DisplayStyle.Modern,
                    PrimaryColor = "#007bff",
                    SecondaryColor = "#6c757d",
                    BackgroundColor = "#ffffff",
                    TextColor = "#333333",
                    FontFamily = "'Segoe UI', Tahoma, Geneva, Verdana, sans-serif"
                }
            };
        }
    }
}