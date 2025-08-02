using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelData.Data;
using ModelData.Models;
using System.Globalization;

namespace AdminUI.Areas.Admin.Pages.Dashboard
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public int TotalPosts { get; set; }
        public int PublishedPosts { get; set; }
        public int ScheduledPosts { get; set; }
        public int UnpublishedPosts { get; set; }

        public int TotalCategories { get; set; }
        public int TotalComments { get; set; }
        public int TotalMediaFiles { get; set; }

        public List<Comment> RecentComments { get; set; } = new();
        public List<ApplicationUser> LastLoggedInUsers { get; set; } = new();

        public Dictionary<string, int> PostsByDay { get; set; } = new();
        public DateTime SelectedDate { get; set; } = DateTime.UtcNow;
        public string FilterType { get; set; } = "Month"; // or "Week" 

        public Setting Setting { get; set; }
        public async Task OnGetAsync(DateTime? selectedDate = null, string? filterType = "")
        {
            Setting = await _context.Settings.FirstOrDefaultAsync();
            SelectedDate = selectedDate ?? DateTime.UtcNow;
            FilterType = string.IsNullOrWhiteSpace(filterType) ? "Month" : filterType;

            var posts = _context.Posts.AsNoTracking().AsQueryable();

            TotalPosts = await posts.CountAsync();
            PublishedPosts = await posts.CountAsync(p => p.IsPublished);
            ScheduledPosts = await posts.CountAsync(p => p.IsScheduled);
            UnpublishedPosts = await posts.CountAsync(p => !p.IsPublished);

            TotalCategories = await _context.Categories.CountAsync();
            TotalComments = await _context.Comments.CountAsync();
            TotalMediaFiles = await _context.MediaFiles.CountAsync();

            RecentComments = await _context.Comments
                .Include(c => c.Post)
                .OrderByDescending(c => c.CreatedAt)
                .Take(5)
                .ToListAsync();

            LastLoggedInUsers = await _context.Users
                .Where(u => u.LastLoginAt != null)
                .OrderByDescending(u => u.LastLoginAt)
                .Take(6)
                .ToListAsync();

            var date = SelectedDate;

            if (FilterType == "Week")
            {
                var monday = date.AddDays(-(int)date.DayOfWeek + (int)DayOfWeek.Monday);
                var sunday = monday.AddDays(7);

                var allWeekDays = Enumerable.Range(0, 7)
                    .Select(offset => monday.AddDays(offset))
                    .ToList();

                var postsInWeek = await posts
                    .Where(p => p.Date >= monday && p.Date < sunday)
                    .ToListAsync();

                PostsByDay = allWeekDays
                    .ToDictionary(
                        day => day.ToString("ddd dd", CultureInfo.InvariantCulture),
                        day => postsInWeek.Count(p => p.Date.Date == day.Date)
                    );
            }
            else if (FilterType == "Month")
            {
                var firstDay = new DateTime(date.Year, date.Month, 1);
                var daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
                var lastDay = firstDay.AddMonths(1);

                var allMonthDays = Enumerable.Range(1, daysInMonth)
                    .Select(day => new DateTime(date.Year, date.Month, day))
                    .ToList();

                var postsInMonth = await posts
                    .Where(p => p.Date >= firstDay && p.Date < lastDay)
                    .ToListAsync();

                PostsByDay = allMonthDays
                    .ToDictionary(
                        day => day.ToString("dd"), // Only the day number
                        day => postsInMonth.Count(p => p.Date.Date == day.Date)
                    );
            }
        }
    }
}