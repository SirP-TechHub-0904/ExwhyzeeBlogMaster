using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelData.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminUI.Areas.Admin.Pages.Users
{
    [Authorize(Policy = "UsersPolicy")]
    public class DetailsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ModelData.Data.ApplicationDbContext _context;

        public DetailsModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ModelData.Data.ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public ApplicationUser AppUser { get; set; }
        public IList<string> UserRoles { get; set; } = new List<string>();
        public DateTime ExpiryDate { get; set; }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            AppUser = await _userManager.FindByIdAsync(id);
            if (AppUser == null)
                return NotFound();

            UserRoles = await _userManager.GetRolesAsync(AppUser);

            return Page();
        }

        public async Task<IActionResult> OnGetGenerateResetLinkAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found!";
                return RedirectToPage("./Index");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            // Save in DB
            var shortLink = new PasswordResetLink
            {
                UserId = user.Id,
                Token = token,
                ExpiryDate = DateTime.UtcNow.AddHours(24) 
            };
            _context.PasswordResetLinks.Add(shortLink);
            await _context.SaveChangesAsync();

            var resetUrl = $"{Request.Scheme}://{Request.Host}/r/{shortLink.ShortCode}";
            TempData["ResetLink"] = resetUrl;
            var xxx = shortLink.ExpiryDate.ToString("f");
            TempData["ExpiryDate"] = shortLink.ExpiryDate.ToString("f");
            return RedirectToPage(new { id });
        }

    }
}
