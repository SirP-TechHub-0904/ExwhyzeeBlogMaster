using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelData.Data;
using ModelData.Models;

namespace AdminUI.Areas.Admin.Pages.Forms
{
    public class ResponsesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ResponsesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public WebPageForm Form { get; set; } = new();
        public Dictionary<string, (string Name, string Email)> UserDetails { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Form = await _context.WebPageForms
                .Include(f => f.Fields)
                .Include(f => f.Responses)
                    .ThenInclude(r => r.FieldValues)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (Form == null)
                return NotFound();
            if (!Form.AllowAnonymousResponses)
            {
                var userIds = Form.Responses
                    .Where(r => !string.IsNullOrEmpty(r.UserId))
                    .Select(r => r.UserId!)
                    .Distinct()
                    .ToList();

                var users = await _context.Users
                    .Where(u => userIds.Contains(u.Id))
                    .Select(u => new { u.Id, u.Email, u.FullName })
                    .ToListAsync();

                UserDetails = users.ToDictionary(u => u.Id, u => (u.FullName ?? "No Name", u.Email ?? "No Email"));
            }

            return Page();
        }
    }
}
