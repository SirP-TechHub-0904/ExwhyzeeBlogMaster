using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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

    public class EditModel : PageModel
    {
        private readonly ModelData.Data.ApplicationDbContext _context;

        public EditModel(ModelData.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public WebPage WebPage { get; set; } = default!;
        public SelectList FormOptions { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id, string? slug)
        {
            if (id == null)
            {
                return NotFound();
            }

            var webpage =  await _context.Pages.FirstOrDefaultAsync(m => m.Id == id);
            if (webpage == null)
            {
                return NotFound();
            }
            WebPage = webpage;
            FormOptions = new SelectList(await _context.WebPageForms.ToListAsync(), "Id", "Title");

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
           
            WebPage.Slug = await GenerateUniqueSlugAsync(WebPage.Title, WebPage.Id);
            _context.Attach(WebPage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WebPageExists(WebPage.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }
        public async Task<string> GenerateUniqueSlugAsync(string title, int? id = null)
        {
            string baseSlug = ModelData.Services.SlugManager.GenerateSlug(title);
            string slug = baseSlug;
            int count = 1;

            while (await _context.Pages
                .AnyAsync(p => p.Slug == slug && (!id.HasValue || p.Id != id)))
            {
                slug = $"{baseSlug}-{count++}";
            }

            return slug;
        }

        private bool WebPageExists(int id)
        {
            return _context.Pages.Any(e => e.Id == id);
        }
    }
}
