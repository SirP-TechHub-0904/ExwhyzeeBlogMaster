using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModelData.Models;
using ModelData.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace AdminUI.Areas.Admin.Pages.Users
{
    public class SeedDefaultAccountsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public SeedDefaultAccountsModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        public List<string> Roles { get; set; } = new();

        public List<(string Email, string Password, string Role, string FullName, string Country, string City)> DefaultAccounts { get; set; } = new()
{
    ("superblog@exwhyzee.ng", "supeR@2025", "SuperAdmin", "Super Admin", "Nigeria", "FCT"),
    ("adminblog@exwhyzee.ng", "adminBlog@2025", "Admin", "Admin", "Nigeria", "FCT")
};



        public async Task OnGetAsync()
        {
            string[] roleNames =
           {
                "Admin",
                "SuperAdmin",
                "PostManager",
                "PageManager",
                "CategoryManager",
                "CommentManager",
                "ImageManager",
                "SettingsManager"
            };

            foreach (var roleName in roleNames)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
                Roles.Add(roleName);
            }
            await EnsureRolesExist();
            await EnsureAccountsExist();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Delete existing accounts
            foreach (var account in DefaultAccounts)
            {
                var user = await _userManager.FindByEmailAsync(account.Email);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }
            }

            // Recreate accounts
            await EnsureRolesExist();
            await EnsureAccountsExist();

            return RedirectToPage("/Settings/Index"); // reload page to show table
        }

        private async Task EnsureRolesExist()
        {
            foreach (var account in DefaultAccounts)
            {
                if (!await _roleManager.RoleExistsAsync(account.Role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(account.Role));
                }
            }
        }

        private async Task EnsureAccountsExist()
        {
            foreach (var account in DefaultAccounts)
            {
                var user = await _userManager.FindByEmailAsync(account.Email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = account.Email,
                        Email = account.Email,
                        EmailConfirmed = true,
                        FullName = account.FullName,
                        City = account.City,
                        Country = account.Country
                    };

                    var result = await _userManager.CreateAsync(user, account.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, account.Role);

                        if (account.Role == "SuperAdmin")
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                        }
                    }
                }
            }
        }
    }
}
