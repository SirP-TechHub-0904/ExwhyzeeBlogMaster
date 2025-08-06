using AdminUI;
using AdminUI.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ModelData;
using ModelData.Data;
using ModelData.Models;
using ModelData.Models;
using ModelData.Services;

namespace AfriRanking
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            //builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false; // <-- disable special characters
                options.Password.RequireLowercase = false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";           // Redirect when not logged in
                options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // Redirect when access denied
                options.ExpireTimeSpan = TimeSpan.FromMinutes(300);
                options.SlidingExpiration = true;
            });
            builder.Services.AddRazorPages();

            builder.Services.AddModelDataServices();
            builder.Services.AddAdminUIServices();
            builder.Services.AddControllersWithViews()
    .AddApplicationPart(typeof(AdminUI.Program).Assembly);
            builder.Services.AddHostedService<BlogWorkerService.PostPublishService>();
            builder.Services.AddScoped<ISystemLogService, SystemLogService>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllers();
            builder.Services.AddControllers()
    .AddApplicationPart(typeof(UniversalsController).Assembly);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();
            app.MapShortLinkRedirect();
            app.UseErrorLogging();
            app.Run();
        }
    }
}
