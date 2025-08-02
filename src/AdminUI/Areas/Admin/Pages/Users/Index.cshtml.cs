using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModelData.Models;
using System.Collections.Generic;
using System.Linq;

namespace AdminUI.Areas.Admin.Pages.Users
{
    [Authorize(Policy = "UsersPolicy")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public List<ApplicationUser> Users { get; set; } = new();
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public string Search { get; set; } = "";

        public async Task OnGetAsync(string? search, int pageNumber = 1)
        {
            Search = search ?? "";
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            int pageSize = 50;

            var allUsers = _userManager.Users.ToList();

            // Filter out SuperAdmins
            var filteredUsers = new List<ApplicationUser>();
            foreach (var user in allUsers)
            {
                if (!await _userManager.IsInRoleAsync(user, "SuperAdmin"))
                {
                    filteredUsers.Add(user);
                }
            }

            // Apply search
            if (!string.IsNullOrWhiteSpace(Search))
            {
                filteredUsers = filteredUsers
                    .Where(u =>
                        (!string.IsNullOrEmpty(u.FullName) && u.FullName.Contains(Search, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(u.Email) && u.Email.Contains(Search, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(u.PhoneNumber) && u.PhoneNumber.Contains(Search)) ||
                        (!string.IsNullOrEmpty(u.City) && u.City.Contains(Search, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(u.Country) && u.Country.Contains(Search, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
            }

            TotalCount = filteredUsers.Count;
            TotalPages = (int)Math.Ceiling(TotalCount / (double)pageSize);

            Users = filteredUsers
                .Skip((PageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

    }
}
