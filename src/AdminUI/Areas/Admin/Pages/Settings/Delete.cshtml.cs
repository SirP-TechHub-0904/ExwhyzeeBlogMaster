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

namespace AdminUI.Areas_Admin_Pages_Settings
{
    [Authorize(Policy = "SettingsPolicy")]

    public class DeleteModel : PageModel
    {
        private readonly ModelData.Data.ApplicationDbContext _context;

        public DeleteModel(ModelData.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Setting Setting { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var setting = await _context.Settings.FirstOrDefaultAsync(m => m.Id == id);

            if (setting is not null)
            {
                Setting = setting;

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

            var setting = await _context.Settings.FindAsync(id);
            if (setting != null)
            {
                Setting = setting;
                _context.Settings.Remove(Setting);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
