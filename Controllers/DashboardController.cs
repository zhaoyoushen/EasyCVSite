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

        public DashboardController(
            UserManager<ApplicationUser> userManager,
            IUserConfigurationService userConfigService,
            ILogger<DashboardController> logger)
        {
            _userManager = userManager;
            _userConfigService = userConfigService;
            _logger = logger;
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
                PublicUrl = !string.IsNullOrEmpty(user.CustomUrl) 
                    ? Url.Action("Public", "Home", new { url = user.CustomUrl }, Request.Scheme)
                    : null,
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

        [HttpGet]
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