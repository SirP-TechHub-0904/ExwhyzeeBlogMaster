using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ModelData.Data;
using ModelData.Models;
using ModelData.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelData
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddModelDataServices(this IServiceCollection services)
        {
            services.AddScoped<ISettingsService, SettingsService>();
            services.AddScoped<IPostServices, PostServices>();
            services.AddScoped<ICategoryServices, CategoryServices>();
            services.AddScoped<IWebPageService, WebPageService>();
            services.AddScoped<IAdvertService, AdvertService>();


            // UploadService registration with scoped dependencies
            services.AddScoped<UploadService>(provider =>
            {
                var context = provider.GetRequiredService<ApplicationDbContext>();
                var env = provider.GetRequiredService<IWebHostEnvironment>(); 
                var setting = context.Settings.FirstOrDefault() ?? new Setting
                {
                    MaxImageSizeKB = 500,
                    MaxVideoSizeKB = 10240
                };

                return new UploadService(env, setting.MaxImageSizeKB, setting.MaxVideoSizeKB, context);
            });

            return services;
        }
    }
}
