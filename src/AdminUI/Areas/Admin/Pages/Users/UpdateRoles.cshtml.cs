using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModelData.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminUI.Areas.Admin.Pages.Users
{
    [Authorize(Policy = "UsersPolicy")]
    public class UpdateRolesModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UpdateRolesModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public ApplicationUser AppUser { get; set; }
        public List<RoleViewModel> Roles { get; set; } = new();

        [BindProperty]
        public List<string> SelectedRoles { get; set; } = new();

        public class RoleViewModel
        {
            public string Name { get; set; }
            public bool IsAssigned { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            AppUser = await _userManager.FindByIdAsync(id);
            if (AppUser == null)
                return NotFound();

            var userRoles = await _userManager.GetRolesAsync(AppUser);
            Roles = _roleManager.Roles.Select(r => new RoleViewModel
            {
                Name = r.Name,
                IsAssigned = userRoles.Contains(r.Name)
            }).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToAdd = SelectedRoles.Except(currentRoles);
            var rolesToRemove = currentRoles.Except(SelectedRoles);

            await _userManager.AddToRolesAsync(user, rolesToAdd);
            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

            return RedirectToPage("Index");
        }
    }
}
