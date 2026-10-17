using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelData.Data;
using ModelData.Models;
using ModelData.Services;

namespace AdminUI.Areas.Admin.Pages.Adverts
{
    [Authorize(Policy = "UsersPolicy")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UploadService _uploadService;
        private readonly IWebHostEnvironment _env;

        public IndexModel(ApplicationDbContext context, UploadService uploadService, IWebHostEnvironment env)
        {
            _context = context;
            _uploadService = uploadService;
            _env = env;
        }

        public List<Advert> Adverts { get; set; } = new();

        [BindProperty]
        public Advert NewAdvert { get; set; } = new();

        [BindProperty]
        public IFormFile? File { get; set; }

        public async Task OnGetAsync()
        {
            Adverts = await _context.Adverts
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (File != null)
            {
                var result = await _uploadService.UploadMediaAsync(File, isVideo: false);
                if (!result.Success)
                {
                    TempData["error"] = result.Error ?? "Failed to upload image.";
                    return RedirectToPage();
                }

                NewAdvert.ImageUrl = result.FilePath;
            }

            NewAdvert.CreatedAt = DateTime.UtcNow;
            _context.Adverts.Add(NewAdvert);
            await _context.SaveChangesAsync();

            TempData["success"] = "Advert created successfully.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var advert = await _context.Adverts.FindAsync(id);
            if (advert == null) return RedirectToPage();

            // Delete the file from wwwroot if exists
            if (!string.IsNullOrEmpty(advert.ImageUrl))
            {
                var filePath = Path.Combine(_env.WebRootPath, advert.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            _context.Adverts.Remove(advert);
            await _context.SaveChangesAsync();

            TempData["success"] = "Advert deleted successfully.";
            return RedirectToPage();
        }
    }
}
