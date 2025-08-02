using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelData.Data;
using ModelData.Models;

namespace AdminUI.Areas.Admin.Pages.Forms
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public WebPageForm Form { get; set; } = new();

        [BindProperty]
        public WebPageFormField NewField { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Form = await _context.WebPageForms
                .Include(f => f.Fields)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (Form == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAddFieldAsync()
        {
           

            _context.WebPageFormFields.Add(NewField);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id = NewField.WebPageFormId });
        }

        public async Task<IActionResult> OnPostDeleteFieldAsync(int fieldId, int formId)
        {
            var field = await _context.WebPageFormFields.FindAsync(fieldId);
            if (field != null)
            {
                _context.WebPageFormFields.Remove(field);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { id = formId });
        }
    }
}
