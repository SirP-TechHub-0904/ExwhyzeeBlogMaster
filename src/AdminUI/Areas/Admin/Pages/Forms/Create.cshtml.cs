using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ModelData.Data;
using ModelData.Models;

namespace AdminUI.Areas.Admin.Pages.Forms
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public WebPageForm Form { get; set; } = new();
        public void OnGet()
        {
           
        }

        public async Task<IActionResult> OnPostAsync()
        {
            

            _context.WebPageForms.Add(Form);
            await _context.SaveChangesAsync();

            return RedirectToPage("Edit", new { id = Form.Id });
        }
    }
}
