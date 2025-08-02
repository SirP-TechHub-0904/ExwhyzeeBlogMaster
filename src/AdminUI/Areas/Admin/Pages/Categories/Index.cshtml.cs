using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelData.Data;
using ModelData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminUI.Areas_Admin_Pages_Categories
{
    [Authorize(Policy = "CategoriesPolicy")]

    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Category> Category { get; set; } = [];

        public async Task<IActionResult> OnGetAsync()
        {
            Category = await _context.Categories
                .Include(c => c.ParentCategory)
               
                .ToListAsync();

            return Page();
        }
    }

}
