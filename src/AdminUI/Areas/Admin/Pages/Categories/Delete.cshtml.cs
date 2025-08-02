using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelData.Models;

namespace AdminUI.Areas_Admin_Pages_Categories
{
    [Authorize(Policy = "CategoriesPolicy")]

    public class DeleteModel : PageModel
    {
        private readonly ModelData.Data.ApplicationDbContext _context;

        public DeleteModel(ModelData.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Category Category { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            Category = await _context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Category == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var categoryToDelete = await _context.Categories
                .Include(c => c.Children)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (categoryToDelete == null)
                return NotFound();

            // 🔹 Prevent deleting if it has child categories
            if (categoryToDelete.Children.Any())
            {
                ModelState.AddModelError(string.Empty, "Cannot delete a category that has child categories.");
                Category = categoryToDelete;
                TempData["error"] = "This category cannot be deleted because it has child categories.";
                return Page();
            }

            // 🔹 Prevent deleting if it has posts
            bool hasPosts = await _context.Posts.AnyAsync(p => p.CategoryId == categoryToDelete.Id);
            if (hasPosts)
            {
                ModelState.AddModelError(string.Empty, "Cannot delete this category because it is used by some posts.");
                Category = categoryToDelete;

                TempData["error"] = "This category cannot be deleted because it is associated with existing posts.";
                return Page();
            }

            _context.Categories.Remove(categoryToDelete);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
