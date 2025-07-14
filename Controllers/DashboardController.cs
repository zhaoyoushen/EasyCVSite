using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PersonalHomepage.Models;
using PersonalHomepage.Services;
using System.Text.Json;

namespace PersonalHomepage.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserConfigurationService _userConfigService;
        private readonly ILogger<DashboardController> _logger;
        private readonly IWebHostEnvironment _environment;

        public DashboardController(
            UserManager<ApplicationUser> userManager,
            IUserConfigurationService userConfigService,
            ILogger<DashboardController> logger,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _userConfigService = userConfigService;
            _logger = logger;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var profile = await _userConfigService.GetUserProfileAsync(user.Id);
            var configuration = await _userConfigService.GetUserConfigurationAsync(user.Id);

            var viewModel = new DashboardViewModel
            {
                UserProfile = configuration,
                UserDisplayName = user.DisplayName ?? user.Email,
                PublicUrl = user.CustomUrl,
                IsPublished = profile?.IsPublished ?? false,
                TotalViews = profile?.ViewCount ?? 0,
                CreatedAt = profile?.CreatedAt ?? DateTime.UtcNow,
                LastUpdatedAt = profile?.UpdatedAt
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var configuration = await _userConfigService.GetUserConfigurationAsync(user.Id);
            return View(configuration);
        }

        [HttpPost]
        public async Task<IActionResult> SaveConfiguration([FromBody] PageConfiguration configuration)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                await _userConfigService.SaveUserConfigurationAsync(user.Id, configuration);
                return Json(new { success = true, message = "Configuration saved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving configuration");
                return Json(new { success = false, message = "Error saving configuration" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Publish()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                if (string.IsNullOrEmpty(user.CustomUrl))
                {
                    return Json(new { success = false, message = "Please set a custom URL first" });
                }

                var result = await _userConfigService.PublishConfigurationAsync(user.Id);
                if (result)
                {
                    var publicUrl = Url.Action("Public", "Home", new { url = user.CustomUrl }, Request.Scheme);
                    return Json(new { success = true, message = "Homepage published successfully", publicUrl });
                }

                return Json(new { success = false, message = "Failed to publish homepage" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing configuration");
                return Json(new { success = false, message = "Error publishing homepage" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Unpublish()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                var result = await _userConfigService.UnpublishConfigurationAsync(user.Id);
                return Json(new { success = result, message = result ? "Homepage unpublished" : "Failed to unpublish" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unpublishing configuration");
                return Json(new { success = false, message = "Error unpublishing homepage" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetCustomUrl([FromBody] SetCustomUrlRequest request)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                // 验证URL格式
                if (string.IsNullOrWhiteSpace(request.CustomUrl) || 
                    !IsValidCustomUrl(request.CustomUrl))
                {
                    return Json(new { success = false, message = "Invalid URL format. Use only letters, numbers, and hyphens." });
                }

                // 检查URL是否可用
                if (!await _userConfigService.IsCustomUrlAvailableAsync(request.CustomUrl, user.Id))
                {
                    return Json(new { success = false, message = "This URL is already taken" });
                }

                var result = await _userConfigService.SetCustomUrlAsync(user.Id, request.CustomUrl);
                if (result)
                {
                    var publicUrl = Url.Action("Public", "Home", new { url = request.CustomUrl }, Request.Scheme);
                    return Json(new { success = true, message = "Custom URL set successfully", publicUrl });
                }

                return Json(new { success = false, message = "Failed to set custom URL" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting custom URL");
                return Json(new { success = false, message = "Error setting custom URL" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckUrlAvailability(string url)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { available = false });
                }

                if (string.IsNullOrWhiteSpace(url) || !IsValidCustomUrl(url))
                {
                    return Json(new { available = false, message = "Invalid URL format" });
                }

                var available = await _userConfigService.IsCustomUrlAvailableAsync(url, user.Id);
                return Json(new { available });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking URL availability");
                return Json(new { available = false });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadProfileImage(IFormFile profileImage)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                _logger.LogInformation($"Received file upload request. ProfileImage is null: {profileImage == null}");
                if (profileImage != null)
                {
                    _logger.LogInformation($"File details - Name: {profileImage.FileName}, Size: {profileImage.Length}, ContentType: {profileImage.ContentType}");
                }

                if (profileImage == null || profileImage.Length == 0)
                {
                    return Json(new { success = false, message = "No file selected" });
                }

                // 验证文件类型
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(profileImage.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return Json(new { success = false, message = "Invalid file type. Only JPG, PNG, GIF, and WebP files are allowed." });
                }

                // 验证文件大小 (最大5MB)
                if (profileImage.Length > 5 * 1024 * 1024)
                {
                    return Json(new { success = false, message = "File size too large. Maximum size is 5MB." });
                }

                // 创建用户专属文件夹
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "profiles", user.Id);
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                // 删除旧的头像文件
                var existingFiles = Directory.GetFiles(uploadsPath, "profile.*");
                foreach (var file in existingFiles)
                {
                    System.IO.File.Delete(file);
                }

                // 生成新文件名
                var fileName = $"profile{fileExtension}";
                var filePath = Path.Combine(uploadsPath, fileName);

                // 保存文件
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profileImage.CopyToAsync(stream);
                }

                // 生成访问URL
                var imageUrl = $"/uploads/profiles/{user.Id}/{fileName}";

                return Json(new { success = true, imageUrl, message = "Profile image uploaded successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading profile image");
                return Json(new { success = false, message = "Error uploading image" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProfileImage()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                // 删除用户头像文件
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "profiles", user.Id);
                if (Directory.Exists(uploadsPath))
                {
                    var existingFiles = Directory.GetFiles(uploadsPath, "profile.*");
                    foreach (var file in existingFiles)
                    {
                        System.IO.File.Delete(file);
                    }
                }

                return Json(new { success = true, message = "Profile image deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking URL availability");
                return Json(new { available = false });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePublishSettings(bool isPublished, string customUrl)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                // 如果要发布但没有自定义URL，返回错误
                if (isPublished && string.IsNullOrWhiteSpace(customUrl))
                {
                    return Json(new { success = false, message = "Please set a custom URL before publishing" });
                }

                // 如果提供了自定义URL，先设置URL
                if (!string.IsNullOrWhiteSpace(customUrl))
                {
                    if (!IsValidCustomUrl(customUrl))
                    {
                        return Json(new { success = false, message = "Invalid URL format. Use only letters, numbers, and hyphens." });
                    }

                    if (!await _userConfigService.IsCustomUrlAvailableAsync(customUrl, user.Id))
                    {
                        return Json(new { success = false, message = "This URL is already taken" });
                    }

                    await _userConfigService.SetCustomUrlAsync(user.Id, customUrl);
                }

                // 设置发布状态
                bool result;
                if (isPublished)
                {
                    result = await _userConfigService.PublishConfigurationAsync(user.Id);
                }
                else
                {
                    result = await _userConfigService.UnpublishConfigurationAsync(user.Id);
                }

                if (result)
                {
                    var message = isPublished ? "Page published successfully!" : "Page unpublished successfully!";
                    return Json(new { success = true, message });
                }

                return Json(new { success = false, message = "Failed to update publish settings" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating publish settings");
                return Json(new { success = false, message = "Error updating publish settings" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadConfiguration(IFormFile configFile)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                if (configFile == null || configFile.Length == 0)
                {
                    return Json(new { success = false, message = "Please select a valid JSON configuration file" });
                }

                var result = await _userConfigService.UploadConfigurationAsync(user.Id, configFile);
                
                if (result)
                {
                    return Json(new { success = true, message = "Configuration uploaded successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to upload configuration. Please check the file format." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading configuration");
                return Json(new { success = false, message = "Error uploading configuration" });
            }
        }

        private static bool IsValidCustomUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            // 只允许字母、数字和连字符，长度3-50
            return url.Length >= 3 && url.Length <= 50 && 
                   url.All(c => char.IsLetterOrDigit(c) || c == '-') &&
                   !url.StartsWith('-') && !url.EndsWith('-');
        }
    }


}