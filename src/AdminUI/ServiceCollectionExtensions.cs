using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminUI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAdminUIServices(this IServiceCollection services)
        {
            // Add any scoped RazorPage-specific logic here
            services.AddAuthorization(options =>
            {
                options.AddPolicy("UsersPolicy", p => p.RequireRole("Admin", "SuperAdmin"));
                options.AddPolicy("PostsPolicy", p => p.RequireRole("Admin", "SuperAdmin", "PostManager"));
                options.AddPolicy("CommentsPolicy", p => p.RequireRole("Admin", "SuperAdmin", "CommentManager"));
                options.AddPolicy("CategoriesPolicy", p => p.RequireRole("Admin", "SuperAdmin", "CategoryManager"));
                options.AddPolicy("MediaFilesPolicy", p => p.RequireRole("Admin", "SuperAdmin", "ImageManager"));
                options.AddPolicy("SettingsPolicy", p => p.RequireRole("Admin", "SuperAdmin", "SettingsManager"));
                options.AddPolicy("WebPagesPolicy", p => p.RequireRole("Admin", "SuperAdmin", "PageManager"));
            });
 
            return services;
        }
    }
}
