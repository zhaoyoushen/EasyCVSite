using PersonalHomepage.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PersonalHomepage.Services
{
    public interface IConfigurationService
    {
        Task<PageConfiguration> GetConfigurationAsync();
        Task SaveConfigurationAsync(PageConfiguration configuration);
        Task<bool> UploadConfigurationAsync(IFormFile file);
        PageConfiguration GetDefaultConfiguration();
    }

    public class ConfigurationService : IConfigurationService
    {
        private readonly string _configFilePath;
        private readonly ILogger<ConfigurationService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public ConfigurationService(IWebHostEnvironment environment, ILogger<ConfigurationService> logger)
        {
            _configFilePath = Path.Combine(environment.ContentRootPath, "Data", "page-config.json");
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            // 确保Data目录存在
            var dataDir = Path.GetDirectoryName(_configFilePath);
            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir!);
            }
        }

        public async Task<PageConfiguration> GetConfigurationAsync()
        {
            try
            {
                if (!File.Exists(_configFilePath))
                {
                    var defaultConfig = GetDefaultConfiguration();
                    await SaveConfigurationAsync(defaultConfig);
                    return defaultConfig;
                }

                var json = await File.ReadAllTextAsync(_configFilePath);
                var configuration = JsonSerializer.Deserialize<PageConfiguration>(json, _jsonOptions);
                return configuration ?? GetDefaultConfiguration();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading configuration file");
                return GetDefaultConfiguration();
            }
        }

        public async Task SaveConfigurationAsync(PageConfiguration configuration)
        {
            try
            {
                var json = JsonSerializer.Serialize(configuration, _jsonOptions);
                await File.WriteAllTextAsync(_configFilePath, json);
                _logger.LogInformation("Configuration saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving configuration file");
                throw;
            }
        }

        public async Task<bool> UploadConfigurationAsync(IFormFile file)
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

                // 保存配置
                await SaveConfigurationAsync(configuration);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading configuration file");
                return false;
            }
        }

        public PageConfiguration GetDefaultConfiguration()
        {
            return new PageConfiguration
            {
                PersonalInfo = new PersonalInfo
                {
                    Name = "Your Name",
                    Title = "Full-Stack Developer",
                    Description = "I am a full-stack developer specializing in .NET and Web technologies.",
                    ProfileImageUrl = "https://via.placeholder.com/400",
                    WelcomeMessage = "Welcome to My Personal Homepage",
                    Highlights = new List<string>
                    {
                        "I love coding, solving complex problems, and continuously learning new technologies."
                    }
                },
                Skills = new List<Skill>
                {
                    new Skill { Name = "C#", Proficiency = 90 },
                    new Skill { Name = "ASP.NET Core", Proficiency = 85 },
                    new Skill { Name = "JavaScript", Proficiency = 80 },
                    new Skill { Name = "HTML/CSS", Proficiency = 85 },
                    new Skill { Name = "SQL", Proficiency = 75 },
                    new Skill { Name = "Docker", Proficiency = 70 }
                },
                Projects = new List<Project>
                {
                    new Project
                    {
                        Id = 1,
                        Name = "E-Commerce Platform",
                        Description = "A complete e-commerce solution developed using ASP.NET Core and Entity Framework.",
                        ImageUrl = "https://via.placeholder.com/300x200",
                        DetailedDescription = "A complete e-commerce solution developed using ASP.NET Core and Entity Framework. This project includes user authentication, product catalog, shopping cart, order processing, and admin dashboard.",
                        Technologies = new List<string> { "ASP.NET Core", "Entity Framework", "SQL Server", "Bootstrap" },
                        CompletedDate = DateTime.Now.AddMonths(-6)
                    },
                    new Project
                    {
                        Id = 2,
                        Name = "Task Management System",
                        Description = "A modern task management application based on .NET Core and Angular.",
                        ImageUrl = "https://via.placeholder.com/300x200",
                        DetailedDescription = "A modern task management application based on .NET Core and Angular. Features include task creation, assignment, status tracking, notifications, and reporting.",
                        Technologies = new List<string> { ".NET Core", "Angular", "TypeScript", "Material UI" },
                        CompletedDate = DateTime.Now.AddMonths(-3)
                    },
                    new Project
                    {
                        Id = 3,
                        Name = "Social Media Analytics Tool",
                        Description = "A social media data analysis platform developed using .NET and various APIs.",
                        ImageUrl = "https://via.placeholder.com/300x200",
                        DetailedDescription = "A social media data analysis platform developed using .NET and various APIs. This tool collects, analyzes, and visualizes social media metrics to help businesses make data-driven decisions.",
                        Technologies = new List<string> { ".NET", "REST APIs", "Chart.js", "Azure" },
                        CompletedDate = DateTime.Now.AddMonths(-1)
                    }
                },
                ContactInfo = new ContactInfo
                {
                    Email = "your.email@example.com",
                    Phone = "+1 (555) 123-4567",
                    Location = "Your City, Country",
                    SocialLinks = new List<SocialLink>
                    {
                        new SocialLink { Platform = "GitHub", Url = "https://github.com/yourusername", Icon = "fab fa-github" },
                        new SocialLink { Platform = "LinkedIn", Url = "https://linkedin.com/in/yourusername", Icon = "fab fa-linkedin" },
                        new SocialLink { Platform = "Twitter", Url = "https://twitter.com/yourusername", Icon = "fab fa-twitter" }
                    }
                },
                Theme = new ThemeSettings
                {
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