using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ModelData.Data;
using ModelData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminUI.Areas_Admin_Pages_WebPages
{
    [Authorize(Policy = "WebPagesPolicy")]

    public class CreateModel : PageModel
    {
        private readonly ModelData.Data.ApplicationDbContext _context;

        public CreateModel(ModelData.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public WebPage WebPage { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {

            // Generate unique slug from title
            WebPage.Slug = await GenerateUniqueSlugAsync(WebPage.Title);

            _context.Pages.Add(WebPage);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public async Task<string> GenerateUniqueSlugAsync(string title, int? id = null)
        {
            string baseSlug = ModelData.Services.SlugManager.GenerateSlug(title);
            string slug = baseSlug;
            int count = 1;

            while (await _context.Pages
                .AnyAsync(p => p.Slug == slug && (!id.HasValue || p.Id != id)))
            {
                slug = $"{baseSlug}-{count++}";
            }

            return slug;
        }

    }
}
