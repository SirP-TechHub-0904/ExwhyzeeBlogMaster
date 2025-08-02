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

    public class DetailsModel : PageModel
    {
        private readonly ModelData.Data.ApplicationDbContext _context;

        public DetailsModel(ModelData.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public WebPage WebPage { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id, string? slug)
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
    }
}
