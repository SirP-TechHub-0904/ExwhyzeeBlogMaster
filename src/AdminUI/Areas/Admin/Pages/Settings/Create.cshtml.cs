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

namespace AdminUI.Areas_Admin_Pages_Settings
{
    [Authorize(Policy = "SettingsPolicy")]

    public class CreateModel : PageModel
    {
        private readonly ModelData.Data.ApplicationDbContext _context;

        public CreateModel(ModelData.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            


            return RedirectToPage("./Index");
        }
    }
}
