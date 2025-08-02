using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModelData.Models;
using System.Threading.Tasks;

namespace AdminUI.Areas.Admin.Pages.Users
{
    [Authorize(Policy = "UsersPolicy")]
    public class UpdateEmailModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UpdateEmailModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public string NewEmail { get; set; }

        public IdentityUser AppUser { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            AppUser = await _userManager.FindByIdAsync(id);
            if (AppUser == null)
                return NotFound();

            NewEmail = AppUser.Email;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            // Check if email is actually changing
            if (user.Email != NewEmail)
            {
                // Set both Email and UserName to the new email
                user.Email = NewEmail;
                user.UserName = NewEmail;

                // Update normalized fields (optional but good practice)
                user.NormalizedEmail = _userManager.NormalizeEmail(NewEmail);
                user.NormalizedUserName = _userManager.NormalizeName(NewEmail);

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToPage("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }
    }
}
