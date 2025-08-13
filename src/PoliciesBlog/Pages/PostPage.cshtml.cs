using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using ModelData.Data;
using ModelData.Models;
using ModelData.Services;

namespace PoliciesBlog.Pages
{
    public class PostPageModel : PageModel
    {
        private readonly IPostServices _postService;
        private readonly IAdvertService _advertService; 
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public PostPageModel(IPostServices postService, IAdvertService advertService, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _postService = postService;
            _advertService = advertService;
            _userManager = userManager;
            _context = context;
        }


        public List<Comment> Comments { get; set; } = new();

        [BindProperty]
        public Comment NewComment { get; set; } = new();
        public Post? Post { get; set; }
        public List<Post> OtherPost { get; set; }
        public Advert? FooterAdvert { get; set; }

        public string ModifiedContentHtml { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string Slug { get; set; } = string.Empty;

        public Post? PreviousPost { get; set; }
        public Post? NextPost { get; set; }

        public string PostUrl => $"{Request.Scheme}://{Request.Host}/post/{Post?.Slug}";

        public async Task<IActionResult> OnGetAsync(string slug)
        {
            if (string.IsNullOrEmpty(slug))
                return NotFound();

            Post = await _postService.GetPostBySlugAsync(slug);
            if (Post == null)
                return NotFound();


            ViewData["Title"] = Post.Title;
            ViewData["MetaTitle"] = Post.Title;
            ViewData["MetaDescription"] = Post.ShortDescription ?? Post.Content;
            ViewData["MetaAuthor"] = Post.PostBy;
            ViewData["MetaPublished"] = Post.PublishedAt?.ToString("o");
            ViewData["MetaUrl"] = $"{Request.Scheme}://{Request.Host}/postpage?slug={Post.Slug}";
            ViewData["MetaKeywords"] = (Post.Tags != null && Post.Tags.Any())
    ? string.Join(", ", Post.Tags)
    : string.Empty;

            // Handle MetaImage (absolute URL)
            var relativeImageUrl = Post.PostImages?.FirstOrDefault(p => p.IsDefault)?.ImageUrl ?? "/assets/default.jpg";
            ViewData["MetaImage"] = relativeImageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                ? relativeImageUrl
                : $"{Request.Scheme}://{Request.Host}{relativeImageUrl}";


            FooterAdvert = await _advertService.GetAdvertByPositionAsync(AdvertPosition.BetweenPosts);

            if (!string.IsNullOrWhiteSpace(Post.Content) && FooterAdvert != null)
            {
                var paragraphs = Post.Content.Split(new[] { "</p>" }, StringSplitOptions.None).ToList();
                int injectIndex = (int)Math.Floor(paragraphs.Count * 0.4);

                if (injectIndex > 0 && injectIndex < paragraphs.Count)
                {
                    paragraphs[injectIndex] += $@"
                        <div class='my-4 text-center'>
                            <a href='{FooterAdvert.Link}' target='_blank'>
                                <img src='{FooterAdvert.ImageUrl}' alt='{FooterAdvert.Title}' class='img-fluid w-100' style='height:200px; object-fit:cover;' />
                            </a>
                        </div>";
                }

                ModifiedContentHtml = string.Join("</p>", paragraphs);
            }
            else
            {
                ModifiedContentHtml = Post?.Content ?? "";
            }
            PreviousPost = await _postService.GetPreviousPostAsync(Post.PublishedAt ?? Post.Date);
            NextPost = await _postService.GetNextPostAsync(Post.PublishedAt ?? Post.Date);
            Comments = await _postService.GetApprovedCommentsByPostIdAsync(Post.Id);


            OtherPost = await _postService.GetPostsByCategory(Post.CategoryId ?? 0); 
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
             

            Post = await _postService.GetPostBySlugAsync(Slug);
            if (Post == null)
                return NotFound();

            var comment = new Comment
            {
                PostId = Post.Id,
                Content = NewComment.Content,
                CreatedAt = DateTime.UtcNow,
                IsApproved = true // You can make it true if auto-approve
            };

            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                comment.UserId = user?.Id;
                comment.AuthorName = user?.FullName ?? user?.UserName ?? "User";
                comment.AuthorEmail = user?.Email;
            }
            else
            {
                comment.AuthorName = NewComment.AuthorName;
                comment.AuthorEmail = NewComment.AuthorEmail;
            }

            await _postService.AddCommentAsync(comment);
            TempData["CommentSuccess"] = "Thank you for your comment! It will be reviewed shortly.";

            return RedirectToPage(new { slug = Slug });
        }
        public async Task<IActionResult> OnGetLoadCommentsAsync(int postId, int skip = 0, int take = 5)
        {
            var comments = await _context.Comments
                .Where(c => c.PostId == postId)
                .OrderByDescending(c => c.CreatedAt)
                .Skip(skip)
                .Take(take)
                .Select(c => new {
                    c.AuthorName,
                    CreatedAt = c.CreatedAt.ToString("MMMM dd, yyyy 'at' hh:mm tt"),
                    c.Content
                })
                .ToListAsync();

            return new JsonResult(comments);
        }

        public async Task<IActionResult> OnGetViewCountAsync(int postId)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == postId);
            if (post == null)
                return new JsonResult(new { success = false });

            post.ViewCount++;
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }



    }
}
