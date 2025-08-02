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
    public class EditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public EditModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public ApplicationUser AppUser { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            AppUser = user;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.FindByIdAsync(AppUser.Id);
            if (user == null)
                return NotFound();

            user.FullName = AppUser.FullName;
            user.PhoneNumber = AppUser.PhoneNumber;
            user.City = AppUser.City;
            user.Country = AppUser.Country;
            user.EmailConfirmed = AppUser.EmailConfirmed;
            user.LockoutEnabled = AppUser.LockoutEnabled;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToPage("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return Page();
        }
    }
}
