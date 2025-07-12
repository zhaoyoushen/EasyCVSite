using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PersonalHomepage.Models;
using PersonalHomepage.Services;
using System.Collections.Generic;

namespace PersonalHomepage.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfigurationService _configurationService;

    public HomeController(ILogger<HomeController> logger, IConfigurationService configurationService)
    {
        _logger = logger;
        _configurationService = configurationService;
    }

    public async Task<IActionResult> Index()
    {
        var configuration = await _configurationService.GetConfigurationAsync();
        return View(configuration);
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
    public IActionResult Admin()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> UploadConfiguration(ConfigurationUpload upload)
    {
        if (ModelState.IsValid && upload.ConfigFile != null)
        {
            var success = await _configurationService.UploadConfigurationAsync(upload.ConfigFile);
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
            TempData["ErrorMessage"] = "请选择一个有效的JSON配置文件。";
        }
        
        return RedirectToAction(nameof(Admin));
    }
    
    [HttpGet]
    public async Task<IActionResult> DownloadConfiguration()
    {
        try
        {
            var configuration = await _configurationService.GetConfigurationAsync();
            var json = System.Text.Json.JsonSerializer.Serialize(configuration, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });
            
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            return File(bytes, "application/json", "page-config.json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading configuration");
            TempData["ErrorMessage"] = "下载配置文件时发生错误。";
            return RedirectToAction(nameof(Admin));
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> ResetToDefault()
    {
        try
        {
            var defaultConfig = _configurationService.GetDefaultConfiguration();
            await _configurationService.SaveConfigurationAsync(defaultConfig);
            TempData["SuccessMessage"] = "配置已重置为默认设置。";
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
