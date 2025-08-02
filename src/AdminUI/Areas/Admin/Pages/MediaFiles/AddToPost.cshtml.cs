using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ModelData.Data;
using ModelData.Models;

namespace AdminUI.Areas.Admin.Pages.MediaFiles
{
    [Authorize(Policy = "MediaFilesPolicy")]

    public class AddToPostModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public AddToPostModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty] public MediaFile Media { get; set; } = new();
        [BindProperty] public int SelectedPostId { get; set; }
        [BindProperty] public bool SetAsDefault { get; set; }

        public List<SelectListItem> PostsDropdown { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Media = await _context.MediaFiles.FindAsync(id) ?? new MediaFile();
            if (Media.Id == 0) return NotFound();

            PostsDropdown = await _context.Posts
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Title
                })
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadPostsDropdown();
                return Page();
            }

            var post = await _context.Posts.Include(p => p.PostImages)
                                           .FirstOrDefaultAsync(p => p.Id == SelectedPostId);
            if (post == null)
                return NotFound();

            var newImage = new PostImage
            {
                PostId = post.Id,
                ImageUrl = Media.Url,
                IsDefault = SetAsDefault
            };

            // ✅ If set as default, remove default from other images
            if (SetAsDefault)
            {
                foreach (var img in post.PostImages)
                    img.IsDefault = false;
            }

            post.PostImages.Add(newImage);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Image attached to post successfully!";
            return RedirectToPage("./Index");
        }

        private async Task LoadPostsDropdown()
        {
            PostsDropdown = await _context.Posts
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Title
                })
                .ToListAsync();
        }
    }
}
