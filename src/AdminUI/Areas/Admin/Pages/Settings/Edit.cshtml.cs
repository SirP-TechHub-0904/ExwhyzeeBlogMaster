using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelData.Data;
using ModelData.Models;
using ModelData.Services;

namespace AdminUI.Areas.Admin.Pages.Settings
{
    [Authorize(Policy = "SettingsPolicy")]

    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UploadService _uploadService;

        public EditModel(ApplicationDbContext context, UploadService uploadService)
        {
            _context = context;
            _uploadService = uploadService;
        }

        [BindProperty]
        public Setting Setting { get; set; } = default!;

        [BindProperty]
        public IFormFile? LogoFile { get; set; }

        [BindProperty]
        public IFormFile? FaviconFile { get; set; }
        [BindProperty]
        public IFormFile? BreadcrumbImageFile { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Setting = await _context.Settings.FirstOrDefaultAsync();

            if (Setting == null)
            {
                Setting = new Setting();
                _context.Settings.Add(Setting);
                await _context.SaveChangesAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var existing = await _context.Settings.FirstOrDefaultAsync();
            if (existing == null) return NotFound();

            // Upload Site Logo
            if (LogoFile != null)
            {
                var result = await _uploadService.UploadMediaAsync(LogoFile, isVideo: false);
                if (!result.Success)
                {
                    ModelState.AddModelError("", result.Error!);
                    return Page();
                }
                Setting.SiteLogoUrl = result.FilePath;
            }
            else
            {
                Setting.SiteLogoUrl = existing.SiteLogoUrl;
            }

            // Upload Favicon
            if (FaviconFile != null)
            {
                var result = await _uploadService.UploadMediaAsync(FaviconFile, isVideo: false);
                if (!result.Success)
                {
                    ModelState.AddModelError("", result.Error!);
                    return Page();
                }
                Setting.FaviconUrl = result.FilePath;
            }
            else
            {
                Setting.FaviconUrl = existing.FaviconUrl;
            }

            // Upload Breadcrumb Image
            if (BreadcrumbImageFile != null)
            {
                var result = await _uploadService.UploadMediaAsync(BreadcrumbImageFile, isVideo: false);
                if (!result.Success)
                {
                    ModelState.AddModelError("", result.Error!);
                    return Page();
                }
                Setting.SiteBreadcrumbImageUrl = result.FilePath;
            }
            else
            {
                Setting.SiteBreadcrumbImageUrl = existing.SiteBreadcrumbImageUrl;
            }


            Setting.Id = existing.Id;
            // Update all other fields
            _context.Entry(existing).CurrentValues.SetValues(Setting);
            await _context.SaveChangesAsync();

            TempData["success"] = "Settings updated successfully.";
            return RedirectToPage("./Edit");
        }
    }
}
