using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelData.Data;
using ModelData.Models;

namespace PeopleNewsBlog.Pages
{
    public class AllCategoryModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AllCategoryModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Category> MainCategories { get; set; } = new();

        public async Task OnGetAsync()
        {
            MainCategories = await _context.Categories
                .Where(c => c.IsPublished && c.ShowInMenu && c.ParentCategoryId == null)
                .Include(c => c.Children.Where(child => child.IsPublished))
                .OrderBy(c => c.SortOrder)
                .ToListAsync();
        }
    }
}
