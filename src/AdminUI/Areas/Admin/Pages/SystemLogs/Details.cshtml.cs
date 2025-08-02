using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelData.Data;
using ModelData.Models;

namespace AdminUI.Areas.Admin.Pages.SystemLogs
{
    [Authorize(Policy = "UsersPolicy")]

    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public SystemLog? Log { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Log = await _context.SystemLogs.FirstOrDefaultAsync(l => l.Id == id);

            if (Log == null)
                return NotFound();

            return Page();
        }
    }
}
