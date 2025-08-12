using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ModelData.Models;
using ModelData.Models;

namespace ModelData.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        private readonly IHttpContextAccessor _httpContextAccessor;
        /// <summary>
        /// /
        /// </summary>
        public DbSet<Category> Categories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<WebPage> Pages { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<MediaFile> MediaFiles { get; set; } // formerly Image
        public DbSet<Setting> Settings { get; set; }
        public DbSet<PostImage> PostImages { get; set; }
        public DbSet<PasswordResetLink> PasswordResetLinks { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }
        public DbSet<NewsletterSubscription> NewsletterSubscriptions { get; set; }
        public DbSet<Advert> Adverts { get; set; }
        public DbSet<WebPageForm> WebPageForms { get; set; }
        public DbSet<WebPageFormField> WebPageFormFields { get; set; }
        public DbSet<WebPageFormResponse> WebPageFormResponses { get; set; }
        public DbSet<WebPageFormFieldValue> WebPageFormFieldValues { get; set; }


        private bool _isLogging = false;

        public override int SaveChanges()
        {
            return SaveChangesAsync().GetAwaiter().GetResult();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (_isLogging) // Prevent recursion
                return await base.SaveChangesAsync(cancellationToken);

            _isLogging = true;

            try
            {
                var username = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
                var fullName = _httpContextAccessor.HttpContext?.User?.FindFirst("FullName")?.Value ?? "";

                var entries = ChangeTracker.Entries()
        .Where(e =>
            e.Entity.GetType() != typeof(SystemLog) &&
            (e.State == EntityState.Added ||
             e.State == EntityState.Modified ||
             e.State == EntityState.Deleted))
        .ToList();


                foreach (var entry in entries)
                {
                    string actionType = entry.State switch
                    {
                        EntityState.Added => "Create",
                        EntityState.Modified => "Update",
                        EntityState.Deleted => "Delete",
                        _ => "Unknown"
                    };

                    // Try to get a meaningful property (Title, Name, AuthorName, SiteTitle, Url, ShortCode)
                    string[] priority = { "Title", "Name", "AuthorName", "SiteTitle", "ImageUrl", "Url", "ShortCode" };

                    var titleProperty = entry.CurrentValues.Properties
                        .Where(p => priority.Contains(p.Name, StringComparer.OrdinalIgnoreCase))
                        .OrderBy(p => Array.IndexOf(priority, p.Name))
                        .FirstOrDefault();


                    string? displayValue = null;

                    if (titleProperty != null)
                    {
                        displayValue = entry.CurrentValues[titleProperty.Name]?.ToString();
                    }

                    var entityName = entry.Entity.GetType().Name;
                    var action = actionType;
                    var recordTitle = displayValue;
                    var idValue = entry.CurrentValues.Properties.Any(p => p.Name == "Id")
                        ? entry.CurrentValues["Id"]?.ToString()
                        : null;

                    // Build details string
                    var details = !string.IsNullOrEmpty(recordTitle)
                        ? $"{action} {entityName} '{recordTitle}'"
                        : $"{action} {entityName}" + (idValue != null ? $" (ID: {idValue})" : "");

                    var ipAddress = _httpContextAccessor.HttpContext?.Request?.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

                    SystemLogs.Add(new SystemLog
                    {
                        ActionType = action,
                        EntityName = entityName,
                        Details = details,
                        Username = username,
                        FullName = fullName,
                        Timestamp = DateTime.UtcNow,
                        IpAddress = ipAddress
                    });

                }

                return await base.SaveChangesAsync(cancellationToken);
            }
            finally
            {
                _isLogging = false;
            }
        }
    }
}
