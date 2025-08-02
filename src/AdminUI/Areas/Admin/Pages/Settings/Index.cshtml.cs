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

    public class IndexModel : PageModel
    {
        private readonly ModelData.Data.ApplicationDbContext _context;

        public IndexModel(ModelData.Data.ApplicationDbContext context)
        {
            _context = context;
        }

         
        public Setting Setting { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            // Try to get the first (and only) Setting
            Setting? existingSetting = await _context.Settings.FirstOrDefaultAsync();

            if (existingSetting == null)
            {
                return RedirectToPage("./Create");
                 
            }

            Setting = existingSetting;
            return Page();
        }
    }
}
