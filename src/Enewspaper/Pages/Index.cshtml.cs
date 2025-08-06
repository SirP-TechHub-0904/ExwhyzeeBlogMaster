using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelData.Models;
using ModelData.Services;


//themelate used.
//<!-- saved from url=(0066)https://htmldemo.net/khobor/khobor/index-2.html -->

namespace Enewspaper.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ISettingsService _settingsService;
        private readonly ModelData.Data.ApplicationDbContext _context;

        public IndexModel(ILogger<IndexModel> logger, ISettingsService settingsService, ModelData.Data.ApplicationDbContext context)
        {
            _logger = logger;
            _settingsService = settingsService;
            _context = context;
        }

        public async Task<IActionResult> OnGet()
        {
            var setting = await _settingsService.GetSettingsAsync();

            if (setting != null)
            {
                if (setting.IsMaintenanceMode) {
                    return RedirectToPage("/Maintenance");
                }

            }
            else
            {
                var existingSetting = new Setting
                {
                    SiteTitle = "My Blog Platform",
                    SiteDescription = "A modern and flexible blogging platform for sharing ideas, stories, and updates.",
                    SiteTagline = "Write. Share. Inspire.",
                    SiteLogoUrl = "/uploads/default-logo.png",
                    FaviconUrl = "/uploads/default-favicon.ico",

                    // === SEO & Meta ===
                    MetaKeywords = "blog, news, stories, articles, updates, platform",
                    MetaDescription = "Welcome to our blogging platform where you can explore engaging content and share your own voice.",
                    MetaAuthor = "Blog Platform",
                    MetaHeaderCode = "<meta name=\"robots\" content=\"index, follow\">"
                };
                _context.Settings.Add(existingSetting);
                await _context.SaveChangesAsync();

                return RedirectToPage("/Users/SeedDefaultAccounts", new {area="Admin"});
            }

            return Page();
        }

        [BindProperty]
        public string NewsletterEmail { get; set; } = string.Empty;

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostSubscribeAsync()
        {
            if (string.IsNullOrWhiteSpace(NewsletterEmail) || !NewsletterEmail.Contains("@"))
                return new JsonResult(new { success = false, message = "Please enter a valid email address." });

            var exists = await _context.NewsletterSubscriptions
                .AnyAsync(n => n.Email.ToLower() == NewsletterEmail.ToLower());

            if (exists)
                return new JsonResult(new { success = false, message = "You're already subscribed." });

            _context.NewsletterSubscriptions.Add(new NewsletterSubscription
            {
                Email = NewsletterEmail,
                SubscribedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, message = "Subscription successful! 🎉" });
        }


    }
}
