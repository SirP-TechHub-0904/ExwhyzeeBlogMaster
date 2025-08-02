using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using ModelData.Models;
using System.Text;

namespace AdminUI.Areas.Admin.Pages.Settings
{
    public class SeedDataModel : PageModel
    {
        private readonly ModelData.Data.ApplicationDbContext _context;

        public SeedDataModel(ModelData.Data.ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult OnGet()
        {
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            var categories = new List<Category>();

            var categoryData = new Dictionary<string, List<string>>
{
    { "Health", new List<string> { "Diseases", "Wellness", "Nutrition", "Mental Health" } },
    { "Fashion", new List<string> { "Men's Fashion", "Women's Fashion", "Trends", "Street Style" } },
    { "Technology", new List<string> { "Gadgets", "Software", "AI", "Startups" } },
    { "Business", new List<string> { "Finance", "Startups", "Investing", "Marketing" } },
    { "Sports", new List<string> { "Football", "Basketball", "Tennis", "Olympics" } },
    { "Entertainment", new List<string> { "Movies", "TV Shows", "Music", "Celebrities" } },
    { "Travel", new List<string> { "Destinations", "Tips", "Hotels", "Adventure" } },
    { "Education", new List<string> { "Schools", "Online Learning", "Exams", "Study Tips" } }
};

            foreach (var (mainTitle, subTitles) in categoryData)
            {
                var main = new Category
                {
                    Title = mainTitle,
                    Slug = mainTitle.ToLower().Replace(" ", "-"),
                    IsPublished = true,
                    ShowInMenu = true,
                    ShowInFooter = true,
                    ShowInHomeMain = true,
                    ShowInHome = true
                };

                categories.Add(main);

                foreach (var subTitle in subTitles)
                {
                    categories.Add(new Category
                    {
                        Title = subTitle,
                        Slug = subTitle.ToLower().Replace(" ", "-"),
                        IsPublished = true,
                        ParentCategory = main,
                        ShowInMenu = false,
                        ShowInFooter = false
                    });
                }
            }

            _context.Categories.AddRange(categories);
            await _context.SaveChangesAsync();

            _context.Pages.AddRange(new List<WebPage>
                {
                    CreatePage("About Us", "about-us", "/default/post1.jpg", true),
                    CreatePage("Terms and Conditions", "terms", "/default/post2.jpg", false),
                    CreatePage("Disclaimer", "disclaimer", "/default/post3.jpg", false),
                    CreatePage("Advert", "advert", "/default/post4.jpg", false),
                    CreatePage("Contact Us", "contact-us", "/default/post5.jpg", false)
                });
            await _context.SaveChangesAsync();


            // Fetch categories
            var listcategories = await _context.Categories.ToListAsync();
            var rand = new Random();
            var posts = new List<Post>();

            // Realistic post titles
            var postTitles = new List<string>
{
    "10 Tips for a Healthier Lifestyle",
    "Top 5 Men's Fashion Trends This Year",
    "How AI is Transforming Everyday Life",
    "The Rise of Women's Entrepreneurship",
    "Premier League 2025/2026 Season Preview",
    "The Best Travel Destinations for 2025",
    "Mastering Personal Finance in Your 30s",
    "How to Build a Startup from Scratch",
    "Street Style Looks That Rocked the Summer",
    "Mental Health: Signs You Shouldn't Ignore",
    "Olympic Highlights: Athletes to Watch",
    "The Ultimate Guide to SEO in 2025",
    "The Future of Education: Hybrid Learning",
    "Best Gadgets to Buy Under $100",
    "What to Eat Before and After a Workout",
    "Red Carpet Looks from the Global Awards",
    "Simple Home Remedies That Actually Work",
    "Why You Should Learn a Second Language",
    "5 Marketing Hacks for Small Businesses",
    "Top Movies to Watch This Weekend",
    "How to Improve Sleep Naturally",
    "Gaming Trends to Watch in 2025",
    "Social Media: Tips for Better Engagement",
    "Back-to-School Tech Essentials",
    "How to Stay Motivated Every Day",
    "The Truth About Keto Diets",
    "Top Mobile Apps for Productivity",
    "The Psychology of First Impressions",
    "Why Local News Still Matters",
    "AI vs. Human Creativity: What’s Next?"
};

            for (int i = 0; i < postTitles.Count; i++)
            {
                bool isScheduled = i >= postTitles.Count - 3; // last 3 are scheduled
                DateTime randomDate = isScheduled
                    ? DateTime.Today.AddDays(rand.Next(1, 10)) // within next 10 days
                    : DateTime.Today.AddDays(-rand.Next(0, 30)); // within last 30 days

                var title = postTitles[i];
                var slug = ModelData.Services.SlugManager.GenerateSlug(title); // Use a utility method to generate slug

                 

                var post = new Post
                {
                    Title = title,
                    Slug = slug,
                    Content = GenerateHtmlContent(title),
                    ShortDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce dapibus, tellus ac cursus commodo, tortor mauris condimentum nibh, ut fermentum massa justo sit amet risus.",
                    Date = randomDate,
                    PublishedAt = randomDate,
                    IsPublished = !isScheduled,
                    IsFeatured = true,
                    IsBreakingNews = i < 4,
                    ShowInHero = i < 4,
                    HeroOrder = i < 4 ? i + 1 : 0,
                    ShowInSurface = i < 5,
                    CommentsEnabled = true,
                    SortOrder = i + 1,
                    ViewCount = rand.Next(100, 5000),
                    PostBy = "Admin",
                    CategoryId = listcategories[rand.Next(listcategories.Count)].Id,
                    PostImages = new List<PostImage>
        {
            new PostImage { ImageUrl = $"/default/news{i + 1}.jpg", IsDefault = true }
        }
                };

                posts.Add(post);
            }

            // Add and save to DB
            _context.Posts.AddRange(posts);
            await _context.SaveChangesAsync();

            // ✅ Fetch only main categories (ParentCategory == null)
            var mainCategories = await _context.Categories
                                               .Where(c => c.ParentCategory == null)
                                               .ToListAsync();



            // ✅ Post titles (you can add more if needed)
            var xpostTitles = new List<string>
    {
        "10 Tips for a Healthier Lifestyle", "Top 5 Men's Fashion Trends This Year",
        "How AI is Transforming Everyday Life", "The Rise of Women's Entrepreneurship",
        "Premier League 2025/2026 Season Preview", "The Best Travel Destinations for 2025",
        "Mastering Personal Finance in Your 30s", "How to Build a Startup from Scratch",
        "Street Style Looks That Rocked the Summer", "Mental Health: Signs You Shouldn't Ignore",
        "Olympic Highlights: Athletes to Watch", "The Ultimate Guide to SEO in 2025",
        "The Future of Education: Hybrid Learning", "Best Gadgets to Buy Under $100",
        "What to Eat Before and After a Workout", "Red Carpet Looks from the Global Awards",
        "Simple Home Remedies That Actually Work", "Why You Should Learn a Second Language",
        "5 Marketing Hacks for Small Businesses", "Top Movies to Watch This Weekend",
        "How to Improve Sleep Naturally", "Gaming Trends to Watch in 2025",
        "Social Media: Tips for Better Engagement", "Back-to-School Tech Essentials",
        "How to Stay Motivated Every Day", "The Truth About Keto Diets",
        "Top Mobile Apps for Productivity", "The Psychology of First Impressions",
        "Why Local News Still Matters", "AI vs. Human Creativity: What’s Next?"
    };
            var extraPosts = new List<Post>();
            foreach (var category in mainCategories)
            {
                for (int i = 0; i < 6; i++) // ✅ Ensure 6 posts for each main category
                {
                    var title = xpostTitles[rand.Next(xpostTitles.Count)];
                    var slug = ModelData.Services.SlugManager.GenerateSlug(title);

                    var randomDate = DateTime.Today.AddDays(-rand.Next(0, 30));

                    extraPosts.Add(new Post
                    {
                        Title = title,
                        Slug = slug,
                        Content = GenerateHtmlContent(title),
                        ShortDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce dapibus, tellus ac cursus commodo, tortor mauris condimentum nibh, ut fermentum massa justo sit amet risus.",
                        Date = randomDate,
                        PublishedAt = randomDate,
                        IsPublished = true,
                        IsFeatured = rand.Next(2) == 0,
                        IsBreakingNews = rand.Next(2) == 0,
                        ShowInHero = rand.Next(2) == 0,
                        HeroOrder = 0,
                        ShowInSurface = true,
                        CommentsEnabled = true,
                        SortOrder = rand.Next(1, 100),
                        ViewCount = rand.Next(100, 5000),
                        PostBy = "Admin",
                        CategoryId = category.Id,
                        PostImages = new List<PostImage>
                {
                    new PostImage { ImageUrl = $"/default/news{rand.Next(1, 10)}.jpg", IsDefault = true }
                }
                    });
                }
            }

            _context.Posts.AddRange(extraPosts);
            await _context.SaveChangesAsync();


            var allPosts = await _context.Posts.ToListAsync();
            var names = new[]
{
    "Emily Thompson", "Michael Johnson", "Sophia Lee", "Daniel Carter", "Isabella Nguyen",
    "James Anderson", "Olivia Garcia", "William Martinez", "Ava Robinson", "Liam Walker",
    "Noah Adams", "Emma Scott", "Benjamin Moore", "Mia Turner", "Elijah Hall",
    "Charlotte Allen", "Lucas Young", "Amelia King", "Logan Wright", "Harper Lopez"
};

            var companies = new[]
            {
    "TechNova", "GreenEdge Inc", "PixelWorks", "BrightPath Ltd", "CloudNest",
    "NovaFusion", "HealthBridge", "DataHive", "FreshGrocer", "UrbanLoop"
};

            var petNames = new[]
            {
    "Milo", "Luna", "Charlie", "Bella", "Max", "Daisy", "Rocky", "Lola", "Leo", "Chloe"
};

            var commentTexts = new[]
            {
    "Really insightful post. Thanks for sharing!",
    "I learned something new today. Keep up the great content!",
    "This aligns perfectly with what we're doing at {company}.",
    "My dog {pet} would love this topic 😂",
    "Very helpful, especially the part about hybrid learning.",
    "Great breakdown! Bookmarked for later.",
    "I've shared this with my team at {company}.",
    "Do you have a follow-up post on this?",
    "I never thought of it this way. Game changer!",
    "This deserves more attention. Thank you!"
};

            var comments = new List<Comment>();
            var rands = new Random();

            foreach (var post in allPosts.Take(5))
            {
                for (int i = 0; i < 2; i++)
                {
                    var name = names[rand.Next(names.Length)];
                    var company = companies[rand.Next(companies.Length)];
                    var pet = petNames[rand.Next(petNames.Length)];

                    var rawContent = commentTexts[rand.Next(commentTexts.Length)];
                    var content = rawContent
                        .Replace("{company}", company)
                        .Replace("{pet}", pet);

                    comments.Add(new Comment
                    {
                        PostId = post.Id,
                        AuthorName = name,
                        AuthorEmail = $"{name.ToLower().Replace(" ", ".")}@{company.ToLower().Replace(" ", "")}.com",
                        Content = content,
                        CreatedAt = DateTime.Now.AddMinutes(-rand.Next(5, 10000)),
                        IsApproved = true
                    });
                }
            }

            _context.Comments.AddRange(comments);
            await _context.SaveChangesAsync();


            TempData["success"] = "Seed data created successfully!";
            TempData["seed"] = "successfully";
            return RedirectToPage();
        }

        private WebPage CreatePage(string title, string slug, string img, bool menu)
        {
            return new WebPage
            {
                Title = title,
                Slug = slug,
                Content = $"<h2>{title}</h2><p>This is the {title.ToLower()} page with default HTML content for display. You can edit this as needed.</p>",
                IsPublished = true,
                ShowInMenu = menu,
                ShowInFooter = true,
                MenuSortOrder = 1,
                SortOrder = 1,
                EnableDirectLink = false,
                ImageUrl = img
            };
        }

        private string GenerateHtmlContent(string title)
        {
            var rand = new Random(); // New random instance
            var builder = new StringBuilder();

            // Title
            builder.Append($"<h2>{title}</h2>");
            builder.Append("<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce dapibus, tellus ac cursus commodo, tortor mauris condimentum nibh, ut fermentum massa justo sit amet risus.</p>");

            // Paragraphs
            for (int i = 0; i < rand.Next(2, 4); i++)
            {
                builder.Append("<p>Curabitur blandit tempus porttitor. Vestibulum id ligula porta felis euismod semper. Donec ullamcorper nulla non metus auctor fringilla. Integer posuere erat a ante venenatis dapibus posuere velit aliquet.</p>");
            }

            // Subheading
            builder.Append("<h3>Key Takeaways</h3>");
            builder.Append("<ul>");
            for (int i = 0; i < rand.Next(3, 5); i++)
            {
                builder.Append($"<li>Insightful point #{i + 1} about the topic.</li>");
            }
            builder.Append("</ul>");

            // More paragraphs
            builder.Append("<h3>Further Discussion</h3>");
            for (int i = 0; i < rand.Next(2, 3); i++)
            {
                builder.Append("<p>Quisque velit nisi, pretium ut lacinia in, elementum id enim. Praesent sapien massa, convallis a pellentesque nec, egestas non nisi.</p>");
            }

            // Conclusion
            builder.Append("<h3>Conclusion</h3>");
            builder.Append($"<p>In conclusion, always remember to explore further, stay curious, and never stop learning. This post titled '{title}' wraps up with encouragement to engage in discussion and share your thoughts.</p>");

            return builder.ToString();
        }

    }
}
 