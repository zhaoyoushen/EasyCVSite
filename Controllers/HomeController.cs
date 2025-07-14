using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PersonalHomepage.Models;
using PersonalHomepage.Services;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace PersonalHomepage.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfigurationService _configurationService;
        private readonly IUserConfigurationService _userConfigurationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(
            ILogger<HomeController> logger, 
            IConfigurationService configurationService,
            IUserConfigurationService userConfigurationService,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _configurationService = configurationService;
            _userConfigurationService = userConfigurationService;
            _userManager = userManager;
        }

        public IActionResult Index()
    {
        // 如果用户已登录，重定向到仪表板
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Dashboard");
        }
        
        // 显示欢迎页面
        return View();
    }
    

    
    [Route("u/{url}")]
    public async Task<IActionResult> Public(string url)
    {
        try
        {
            var configuration = await _userConfigurationService.GetPublicConfigurationAsync(url);
            if (configuration == null)
            {
                return NotFound("Homepage not found or not published.");
            }
            
            // 增加访问计数
            await _userConfigurationService.IncrementViewCountAsync(url);
            
            // 为用户自己的页面设置导航栏显示逻辑
            ViewBag.HasSkills = configuration?.Skills?.Any() == true;
            ViewBag.HasProjects = configuration?.Projects?.Any() == true;
            ViewBag.IsPublicView = true;
            ViewBag.CustomUrl = url;
            return View("PublicProfile", configuration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading public profile for URL: {Url}", url);
            return NotFound("Homepage not found.");
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }
    
    public IActionResult Contact()
    {
        return View();
    }
    
    [HttpPost]
    public IActionResult Contact(ContactForm contactForm)
    {
        if (ModelState.IsValid)
        {
            // 这里可以添加发送邮件的逻辑
            // 为了演示，我们只记录一条消息
            _logger.LogInformation($"收到来自 {contactForm.Email} 的联系请求");
            
            // 添加一个临时成功消息
            TempData["SuccessMessage"] = "您的消息已成功发送！我们会尽快回复您。";
            
            return RedirectToAction(nameof(Contact));
        }
        
        return View(contactForm);
    }

    public async Task<IActionResult> ProjectDetails(int id)
    {
        var configuration = await _configurationService.GetConfigurationAsync();
        var project = configuration.Projects?.FirstOrDefault(p => p.Id == id);
        
        if (project == null)
        {
            return RedirectToAction(nameof(Index));
        }
        
        return View(project);
    }
    
    // 配置管理相关方法
    [Authorize]
    public IActionResult Admin()
    {
        return View();
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UploadConfiguration(ConfigurationUpload upload)
    {
        if (ModelState.IsValid && upload.ConfigFile != null)
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                var success = await _userConfigurationService.UploadConfigurationAsync(userId, upload.ConfigFile);
                if (success)
                {
                    TempData["SuccessMessage"] = "配置文件上传成功！页面内容已更新。";
                }
                else
                {
                    TempData["ErrorMessage"] = "配置文件上传失败，请检查文件格式是否正确。";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "用户身份验证失败，请重新登录。";
            }
        }
        else
        {
            TempData["ErrorMessage"] = "请选择一个有效的JSON配置文件。";
        }
        
        return RedirectToAction(nameof(Admin));
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> DownloadConfiguration()
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                var configuration = await _userConfigurationService.GetUserConfigurationAsync(userId);
                var json = System.Text.Json.JsonSerializer.Serialize(configuration, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                });
                
                var bytes = System.Text.Encoding.UTF8.GetBytes(json);
                return File(bytes, "application/json", "page-config.json");
            }
            else
            {
                TempData["ErrorMessage"] = "用户身份验证失败，请重新登录。";
                return RedirectToAction(nameof(Admin));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading configuration");
            TempData["ErrorMessage"] = "下载配置文件时发生错误。";
            return RedirectToAction(nameof(Admin));
        }
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ResetToDefault()
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                // 创建一个空的默认配置
                var defaultConfig = new PageConfiguration
                {
                    PersonalInfo = new PersonalInfo
                    {
                        Name = "Your Name",
                        Title = "Professional Title",
                        Description = "Tell the world about yourself and your expertise.",
                        ProfileImageUrl = "https://via.placeholder.com/400",
                        WelcomeMessage = "Welcome to My Personal Homepage",
                        Highlights = new List<string> { "Add your professional highlights here." }
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
                await _userConfigurationService.SaveUserConfigurationAsync(userId, defaultConfig);
                TempData["SuccessMessage"] = "配置已重置为默认设置。";
            }
            else
            {
                TempData["ErrorMessage"] = "用户身份验证失败，请重新登录。";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting configuration");
            TempData["ErrorMessage"] = "重置配置时发生错误。";
        }
        
        return RedirectToAction(nameof(Admin));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    }
}
