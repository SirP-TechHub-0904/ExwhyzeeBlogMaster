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

namespace AdminUI.Areas_Admin_Pages_WebPages
{
    [Authorize(Policy = "WebPagesPolicy")]

    public class DeleteModel : PageModel
    {
        private readonly ModelData.Data.ApplicationDbContext _context;

        public DeleteModel(ModelData.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public WebPage WebPage { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var webpage = await _context.Pages.FirstOrDefaultAsync(m => m.Id == id);

            if (webpage is not null)
            {
                WebPage = webpage;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var webpage = await _context.Pages.FindAsync(id);
            if (webpage != null)
            {
                WebPage = webpage;
                _context.Pages.Remove(WebPage);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
