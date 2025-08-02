using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelData.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogWorkerService
{
    public class PostPublishService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public PostPublishService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckAndPublishPostsAsync();
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Runs every 1 min
            }
        }

        private async Task CheckAndPublishPostsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var postsToPublish = await db.Posts
                .Where(p => p.IsScheduled && p.PublishedAt <= DateTime.UtcNow)
                .ToListAsync();

            foreach (var post in postsToPublish)
            {
                post.IsPublished = true;
                post.IsScheduled = false;
            }

            if (postsToPublish.Any())
                await db.SaveChangesAsync();
        }
    }
}
