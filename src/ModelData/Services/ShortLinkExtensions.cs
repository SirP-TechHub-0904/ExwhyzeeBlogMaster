using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ModelData.Data;
using ModelData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelData.Services
{
    public static class ShortLinkExtensions
    {
        public static void MapShortLinkRedirect(this WebApplication app)
        {
            app.MapGet("/r/{code}", async (string code, ApplicationDbContext db, UserManager<ApplicationUser> userManager) =>
            {
                var record = await db.PasswordResetLinks.FirstOrDefaultAsync(x => x.ShortCode == code);

                if (record == null || record.ExpiryDate < DateTime.UtcNow)
                    return Results.Redirect("/LinkExpired");

                var user = await userManager.FindByIdAsync(record.UserId);
                if (user == null)
                    return Results.Redirect("/LinkExpired");

                var encodedToken = Uri.EscapeDataString(record.Token);
                return Results.Redirect($"/ResetPassword?email={Uri.EscapeDataString(user.Email)}&code={encodedToken}");
            });
        }
    }
}
