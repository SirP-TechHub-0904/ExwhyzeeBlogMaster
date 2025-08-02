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

    public class DetailsModel : PageModel
    {
        private readonly ModelData.Data.ApplicationDbContext _context;

        public DetailsModel(ModelData.Data.ApplicationDbContext context)
        {
            _context = context;
        }

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
    }
}
