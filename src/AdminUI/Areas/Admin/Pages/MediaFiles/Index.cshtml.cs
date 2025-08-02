using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelData.Data;
using ModelData.Models;
using ModelData.Services;

namespace AdminUI.Areas_Admin_Pages_MediaFiles
{
    [Authorize(Policy = "MediaFilesPolicy")]

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

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize = 12;

        public List<MediaFile> MediaFiles { get; set; } = new();
        public int TotalCount { get; set; }

        public async Task OnGetAsync()
        {
            TotalCount = await _context.MediaFiles.CountAsync();
            MediaFiles = await _context.MediaFiles
                .OrderByDescending(m => m.UploadedAt)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }

        [BindProperty]
        public List<IFormFile> Files { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            if (Files == null || !Files.Any())
            {
                TempData["info"] = "No files selected.";
                return Page();
            }

            int successCount = 0;
            int failCount = 0;

            foreach (var file in Files)
            {
                var result = await _uploadService.UploadMediaAsync(file, isVideo: false);

                if (result.Success)
                    successCount++;
                else
                    failCount++;
            }

            TempData["success"] = $"{successCount} file(s) uploaded successfully, {failCount} failed.";

            return RedirectToPage(); // reload page to see updated list
        }
        public IActionResult OnPostDelete(int id)
        {
            var media = _context.MediaFiles.Find(id);
            if (media == null)
                return RedirectToPage();

            // Delete the file from wwwroot
            var filePath = Path.Combine(_env.WebRootPath, media.Url.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            // Remove from database
            _context.MediaFiles.Remove(media);
            _context.SaveChanges();
            TempData["success"] = "Deleted";
            return RedirectToPage(new { PageNumber });
        }
    }
}
