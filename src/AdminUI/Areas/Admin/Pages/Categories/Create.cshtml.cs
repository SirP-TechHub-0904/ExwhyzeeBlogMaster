using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ModelData.Data;
using ModelData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminUI.Areas_Admin_Pages_Categories
{
    [Authorize(Policy = "CategoriesPolicy")]

    public class CreateModel : PageModel
    {
        private readonly ModelData.Data.ApplicationDbContext _context;

        public CreateModel(ModelData.Data.ApplicationDbContext context)
        {
            _context = context;
        }


        [BindProperty]
        public Category Category { get; set; } = default!;

        public SelectList? ParentOptions { get; set; }

        public void OnGet()
        {
            ParentOptions = new SelectList(_context.Categories, "Id", "Title");
        }

        public async Task<IActionResult> OnPostAsync()
        {
           
            Category.Slug = GenerateSlug(Category.Title);
            _context.Categories.Add(Category);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
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
