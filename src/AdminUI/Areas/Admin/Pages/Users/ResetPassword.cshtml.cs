using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModelData.Models;
using ModelData.Models;
using System.Threading.Tasks;

namespace AdminUI.Areas.Admin.Pages.Users
{
    [Authorize(Policy = "UsersPolicy")]
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ResetPasswordModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public string NewPassword { get; set; }

        public IdentityUser AppUser { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            AppUser = await _userManager.FindByIdAsync(id);
            if (AppUser == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                ModelState.AddModelError("", "Password cannot be empty");
                return Page();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            // Remove old password and set a new one
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, NewPassword);

            if (result.Succeeded)
                return RedirectToPage("Index");

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return Page();
        }
    }
}
