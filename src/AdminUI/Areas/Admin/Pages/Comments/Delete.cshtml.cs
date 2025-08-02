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

namespace AdminUI.Areas_Admin_Pages_Comments
{
    [Authorize(Policy = "CommentsPolicy")]

    public class DeleteModel : PageModel
    {
        private readonly ModelData.Data.ApplicationDbContext _context;

        public DeleteModel(ModelData.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Comment Comment { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.FirstOrDefaultAsync(m => m.Id == id);

            if (comment is not null)
            {
                Comment = comment;

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

            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                Comment = comment;
                _context.Comments.Remove(Comment);
                await _context.SaveChangesAsync();
                TempData["success"] = "Comment deleted successfully.";
            }

            return RedirectToPage("./Index");
        }
    }
}
