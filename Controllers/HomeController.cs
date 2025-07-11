using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PersonalHomepage.Models;
using System.Collections.Generic;

namespace PersonalHomepage.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        // 创建技能列表
        var skills = new List<Skill>
        {
            new Skill { Name = "C#", Proficiency = 90 },
            new Skill { Name = "ASP.NET Core", Proficiency = 85 },
            new Skill { Name = "JavaScript", Proficiency = 80 },
            new Skill { Name = "HTML/CSS", Proficiency = 85 },
            new Skill { Name = "SQL", Proficiency = 75 },
            new Skill { Name = "Docker", Proficiency = 70 }
        };

        return View(skills);
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

    public IActionResult ProjectDetails(int id)
    {
        // 这里可以根据id获取项目详情
        // 为了演示，我们返回一个简单的ViewData
        ViewData["ProjectId"] = id;
        
        switch(id)
        {
            case 1:
                ViewData["ProjectName"] = "E-Commerce Platform";
                ViewData["ProjectDescription"] = "A complete e-commerce solution developed using ASP.NET Core and Entity Framework. This project includes user authentication, product catalog, shopping cart, order processing, and admin dashboard.";
                break;
            case 2:
                ViewData["ProjectName"] = "Task Management System";
                ViewData["ProjectDescription"] = "A modern task management application based on .NET Core and Angular. Features include task creation, assignment, status tracking, notifications, and reporting.";
                break;
            case 3:
                ViewData["ProjectName"] = "Social Media Analytics Tool";
                ViewData["ProjectDescription"] = "A social media data analysis platform developed using .NET and various APIs. This tool collects, analyzes, and visualizes social media metrics to help businesses make data-driven decisions.";
                break;
            default:
                return RedirectToAction(nameof(Index));
        }
        
        return View();
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
