using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ModelData.Data;
using ModelData.Models;

namespace AdminUI.Areas_Admin_Pages_Categories
{
    [Authorize(Policy = "CategoriesPolicy")]

    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Category Category { get; set; } = null!;

        public SelectList ParentCategories { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id, string? slug)
        {
            Category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Category == null)
                return NotFound();

            ParentCategories = new SelectList(
                await _context.Categories
                    .Where(c => c.Id != id)
                    .OrderBy(c => c.Title)
                    .ToListAsync(),
                "Id", "Title");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
             
            Category.Slug = GenerateSlug(Category.Title);

            _context.Attach(Category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(Category.Id))
                    return NotFound();
                else
                    throw;
            }
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }

        private async Task LoadParentCategoriesAsync(int currentId)
        {
            var parents = await _context.Categories
                .Where(c => c.Id != currentId)
                .OrderBy(c => c.Title)
                .ToListAsync();

            ParentCategories = new SelectList(parents, "Id", "Title");
        }

        private string GenerateSlug(string title)
        {
            string baseSlug = ModelData.Services.SlugManager.GenerateSlug(title);
            string slug = baseSlug;
            int count = 1;
            while (_context.Categories.Any(c => c.Slug == slug))
            {
                slug = $"{baseSlug}-{count++}";
            }
            return slug;
        }
    }
}
