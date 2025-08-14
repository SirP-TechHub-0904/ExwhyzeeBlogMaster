using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ModelData.Data;
using ModelData.Models;
using ModelData.Services;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace AdminUI.Areas.Admin.Pages.Posts
{
    [Authorize(Policy = "PostsPolicy")]

    public class ManagePostModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UploadService _uploadService;
        private readonly GeminiService _geminiService;
        private readonly ISettingsService _settingsService;
        private readonly IHttpClientFactory _httpClientFactory;
        public ManagePostModel(ApplicationDbContext context, UploadService uploadService, GeminiService geminiService, ISettingsService settingsService, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _uploadService = uploadService;
            _geminiService = geminiService;
            _settingsService = settingsService;
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty] public Post Post { get; set; } = new();
        [BindProperty] public IFormFileCollection? ImageFiles { get; set; }
        public List<Category> Categories { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id, string? slug)
        {
            await LoadCategoriesAsync();

            if (id.HasValue)
            {
                Post = await _context.Posts
                    .Include(p => p.PostImages)
                    .Include(p => p.Comments)
        .ThenInclude(c => c.User)
                    .FirstOrDefaultAsync(p => p.Id == id.Value) ?? new Post();
            }

            return Page();
        }

        // ? Create or Edit Post (Tab 1)
        public async Task<IActionResult> OnPostSavePostAsync()
        {
            await LoadCategoriesAsync();



            // ? Always regenerate slug before saving
            var newSlug = await GenerateUniqueSlugAsync(Post.Title, Post.Id == 0 ? null : Post.Id);

            if (Post.Id == 0)
            {
                // ? Create new post
                Post.Slug = newSlug;
                Post.PublishedAt = DateTime.UtcNow;
                Post.PostBy = User.FindFirst("FullName")?.Value
                              ?? User.Identity?.Name
                              ?? "Unknown User"; _context.Posts.Add(Post);
                TempData["success"] = "Post created successfully.";
            }
            else
            {
                // ? Update existing post without overwriting other fields
                var existingPost = await _context.Posts.FindAsync(Post.Id);
                if (existingPost == null)
                {
                    TempData["Error"] = "Post not found.";
                    return RedirectToPage("./Index");
                }

                existingPost.Title = Post.Title;
                existingPost.Content = Post.Content;
                existingPost.ShortDescription = Post.ShortDescription;
                existingPost.Slug = newSlug;

                _context.Update(existingPost);
                TempData["success"] = "Post updated successfully.";
            }

            await _context.SaveChangesAsync();

            // ? Redirect to Tab 2 (Images)
            return RedirectToPage(new { id = Post.Id == 0 ? Post.Id : Post.Id, tab = "images" });
        }


        // ? Upload Images (Tab 2)
        public async Task<IActionResult> OnPostUploadImagesAsync(int id)
        {
            Post = await _context.Posts
                .Include(p => p.PostImages)
                .FirstOrDefaultAsync(p => p.Id == id);
            await LoadCategoriesAsync();

            if (Post == null)
            {
                TempData["Error"] = "Post not found.";
                return RedirectToPage(new { id, tab = "post" });
            }

            if (ImageFiles == null || !ImageFiles.Any())
            {
                TempData["Error"] = "Please select at least one image to upload.";
                return RedirectToPage(new { id, tab = "images" });
            }

            bool hasExistingImages = Post.PostImages != null && Post.PostImages.Any();
            int uploadCount = 0;

            foreach (var file in ImageFiles)
            {
                var result = await _uploadService.UploadMediaAsync(file, isVideo: false);
                if (result.Success)
                {
                    Post.PostImages!.Add(new PostImage
                    {
                        ImageUrl = result.FilePath,
                        IsDefault = !hasExistingImages && Post.PostImages.Count == 0 // ? First image becomes default
                    });
                    uploadCount++;
                }
                else
                {
                    TempData["Error"] = $"Failed to upload {file.FileName}: {result.Error}";
                }
            }

            if (uploadCount > 0)
            {
                await _context.SaveChangesAsync();
                TempData["success"] = $"{uploadCount} image(s) uploaded successfully.";
            }

            return RedirectToPage(new { id, tab = uploadCount > 0 ? "settings" : "images" });
        }


        // ? Set Default Image
        public async Task<IActionResult> OnPostSetDefaultImageAsync(int imageId)
        {
            var image = await _context.PostImages.Include(i => i.Post).FirstOrDefaultAsync(i => i.Id == imageId);
            if (image == null)
            {
                TempData["Error"] = "Image not found.";
                return RedirectToPage();
            }

            var postImages = await _context.PostImages.Where(i => i.PostId == image.PostId).ToListAsync();
            foreach (var img in postImages)
                img.IsDefault = img.Id == imageId;

            await _context.SaveChangesAsync();

            TempData["success"] = "Default image updated successfully.";
            return RedirectToPage(new { id = image.PostId, tab = "images" });
        }

        // ? Remove Image
        public async Task<IActionResult> OnPostRemoveImageAsync(int imageId)
        {
            var image = await _context.PostImages.FirstOrDefaultAsync(i => i.Id == imageId);
            if (image == null)
            {
                TempData["Error"] = "Image not found.";
                return RedirectToPage();
            }

            int? postId = image.PostId;

            _context.PostImages.Remove(image);
            await _context.SaveChangesAsync();

            TempData["success"] = "Image removed successfully.";
            return RedirectToPage(new { id = postId, tab = "images" });
        }
        [BindProperty]
        public string TagsJson { get; set; } = "";

        // ? Save Settings (Tab 3)
        public async Task<IActionResult> OnPostSaveSettingsAsync()
        {
            await LoadCategoriesAsync();

            if (Post.CategoryId == null)
            {
                TempData["Error"] = "Please select a category before saving.";
                ModelState.AddModelError("Post.CategoryId", "Category is required.");
                return Page();
            }
            var postToUpdate = await _context.Posts.FindAsync(Post.Id);
            if (postToUpdate == null)
            {
                TempData["Error"] = "Post not found.";
                return RedirectToPage(new { id = Post.Id, tab = "settings" });
            }
            // ✅ Deserialize Tags
            var tags = string.IsNullOrWhiteSpace(TagsJson)
        ? new List<string>()
        : JsonSerializer.Deserialize<List<string>>(TagsJson) ?? new List<string>();

            postToUpdate.Tags = tags;
            // ? Update only settings
            postToUpdate.CategoryId = Post.CategoryId;
            postToUpdate.IsFeatured = Post.IsFeatured;
            postToUpdate.IsBreakingNews = Post.IsBreakingNews;
            postToUpdate.ShowInHero = Post.ShowInHero;
            postToUpdate.HeroOrder = Post.HeroOrder;
            postToUpdate.ShowInSurface = Post.ShowInSurface;
            postToUpdate.CommentsEnabled = Post.CommentsEnabled;
            postToUpdate.SortOrder = Post.SortOrder;
            postToUpdate.PostBy = User.FindFirst("FullName")?.Value
              ?? User.Identity?.Name
              ?? "Unknown User";
            _context.Attach(postToUpdate).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            TempData["success"] = "Post settings updated successfully.";
            return RedirectToPage(new { id = Post.Id, tab = "settings" });
        }

        public async Task<string> GenerateUniqueSlugAsync(string title, int? id = null)
        {
            string baseSlug = ModelData.Services.SlugManager.GenerateSlug(title);
            string slug = baseSlug;
            int count = 1;

            while (await _context.Posts
                .AnyAsync(p => p.Slug == slug && (!id.HasValue || p.Id != id)))
            {
                slug = $"{baseSlug}-{count++}";
            }

            return slug;
        }


        public List<SelectListItem> CategoryDropdown { get; set; } = new();

        public async Task LoadCategoriesAsync()
        {
            var allCategories = await _context.Categories
                .Include(c => c.Children)
                .ToListAsync();

            // Get only root categories (ParentCategoryId == null)
            var rootCategories = allCategories.Where(c => c.ParentCategoryId == null).ToList();

            CategoryDropdown = BuildCategoryDropdown(rootCategories, 0);
        }

        private List<SelectListItem> BuildCategoryDropdown(List<Category> categories, int level)
        {
            var list = new List<SelectListItem>();

            foreach (var cat in categories.OrderBy(c => c.Title))
            {
                list.Add(new SelectListItem
                {
                    Value = cat.Id.ToString(),
                    Text = $"{new string('—', level * 2)} {cat.Title}" // Indent with —
                });

                if (cat.Children != null && cat.Children.Any())
                {
                    list.AddRange(BuildCategoryDropdown(cat.Children.ToList(), level + 1));
                }
            }

            return list;
        }
        public async Task<IActionResult> OnPostDeleteCommentAsync(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
            {
                TempData["Error"] = "Comment not found.";
                return RedirectToPage();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            TempData["success"] = "Comment deleted successfully.";
            return RedirectToPage(new { id = comment.PostId, tab = "comments" });
        }
        public async Task<IActionResult> OnPostToggleCommentApprovalAsync(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment != null)
            {
                comment.IsApproved = !comment.IsApproved; // ✅ Toggle
                await _context.SaveChangesAsync();

                TempData["success"] = comment.IsApproved
                    ? "Comment approved successfully!"
                    : "Comment disapproved successfully!";
            }

            return RedirectToPage(new { id = Post.Id, tab = "comments" });
        }
        public async Task<IActionResult> OnPostTogglePublishAsync(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null) return NotFound();

            // Toggle publish status
            if (post.IsPublished)
            {
                post.IsPublished = false;
                post.PublishedAt = null;
                TempData["success"] = "Post unpublished successfully!";
            }
            else
            {
                post.IsPublished = true;
                post.PublishedAt = DateTime.UtcNow;
                TempData["success"] = "Post published successfully!";
            }

            await _context.SaveChangesAsync();
            return RedirectToPage(new { id = post.Id, tab = "" });
        }

        public async Task<IActionResult> OnPostSchedulePublishAsync(int postId, DateTime scheduleDate)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null) return NotFound();

            post.IsPublished = false; // Not published yet
            post.PublishedAt = scheduleDate.ToUniversalTime();
            post.IsScheduled = true; // Mark as scheduled

            await _context.SaveChangesAsync();
            TempData["success"] = $"Post scheduled for {scheduleDate}.";
            return RedirectToPage(new { id = Post.Id, tab = "" });

        }
        public async Task<IActionResult> OnPostCancelScheduleAsync(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null) return NotFound();

            // Reset schedule info
            post.IsScheduled = false;
            post.PublishedAt = null;

            await _context.SaveChangesAsync();

            TempData["success"] = "Post schedule has been cancelled.";
            return RedirectToPage(new { id = post.Id, tab = "post" });
        }
        public async Task<IActionResult> OnGetGenerateAiContent(string prompt, string tone, int wordCount)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                return new JsonResult(new { message = "Prompt is required" })
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            // Format the full prompt
            string fullPrompt = $"Write an article in {tone} tone, about '{prompt}', around {wordCount} words.";

            try
            {
                AiContentResult result = await _geminiService.GenerateContentAsync(prompt);
                if (result.IsError)
                {
                    return new JsonResult(new { message = result.ErrorMessage })
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                return new JsonResult(new
                {
                    Title = result.Title,
                    ShortDescription = result.ShortDescription,
                    Content = result.FullContentHtml
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    message = "Failed to generate content",
                    error = ex.Message
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

        }

    }
    public class AiRequestModel
    {
        public string Prompt { get; set; }
        public string Tone { get; set; }
        public int WordCount { get; set; }
    }

    public class AiRequest
    {
        public string Prompt { get; set; }
        public string Tone { get; set; }
        public int WordCount { get; set; }
    }

    public class AiResponse
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }
}

