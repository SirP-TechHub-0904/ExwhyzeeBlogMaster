using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelData.Data;
using ModelData.Models;

namespace AfriRanking.Pages
{
    public class WebpageModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public WebpageModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string Slug { get; set; } = string.Empty;

        public WebPage? PageContent { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Slug)) return NotFound();

            PageContent = await _context.Pages
                .Include(p => p.Form)
        .ThenInclude(f => f.Fields)
                .FirstOrDefaultAsync(p => p.Slug == Slug && p.IsPublished);

            if (PageContent == null) return NotFound();

            // If EnableDirectLink is true, redirect
            if (PageContent.EnableDirectLink && !string.IsNullOrWhiteSpace(PageContent.DirectLinkUrl))
            {
                if (PageContent.DirectLinkOpenInNewTab)
                    return RedirectPermanent(PageContent.DirectLinkUrl); // opens same tab
                else
                    return Redirect(PageContent.DirectLinkUrl);
            }

           

            // Meta tags
            var request = HttpContext.Request;
            string baseUrl = $"{request.Scheme}://{request.Host}";
            string imageUrl = PageContent.ImageUrl ?? "/assets/default.jpg";
            string fullImageUrl = imageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                ? imageUrl
                : $"{baseUrl}{imageUrl}";

            ViewData["Title"] = PageContent.Title;
            ViewData["MetaTitle"] = PageContent.Title;
            ViewData["MetaDescription"] = PageContent.Content?.Substring(0, Math.Min(160, PageContent.Content.Length)) ?? "";
            ViewData["MetaAuthor"] = "Admin";
            ViewData["MetaPublished"] = DateTime.UtcNow.ToString("o"); // Or a field if available
            ViewData["MetaImage"] = fullImageUrl;
            ViewData["MetaUrl"] = $"{baseUrl}/webpage?slug={PageContent.Slug}";
            return Page();
        }

        public async Task<IActionResult> OnPostSubmitFormAsync()
        {
            if (string.IsNullOrEmpty(Slug)) return NotFound();

            PageContent = await _context.Pages
                .Include(p => p.Form)
                    .ThenInclude(f => f.Fields)
                .FirstOrDefaultAsync(p => p.Slug == Slug && p.IsPublished);

            if (PageContent == null || PageContent.Form == null)
                return NotFound();

            var form = PageContent.Form;

            // If anonymous not allowed and user not logged in
            if (!form.AllowAnonymousResponses && !User.Identity.IsAuthenticated)
                return Unauthorized();

            var response = new WebPageFormResponse
            {
                WebPageFormId = form.Id,
                SubmittedAt = DateTime.UtcNow,
                UserId = User.Identity.IsAuthenticated ? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value : null,
                FieldValues = new List<WebPageFormFieldValue>()
            };

            foreach (var field in form.Fields)
            {
                var formKey = $"field_{field.Id}";
                var value = Request.Form[formKey];

                if (field.IsRequired && string.IsNullOrWhiteSpace(value))
                {
                    ModelState.AddModelError(string.Empty, $"Field '{field.Label}' is required.");
                    return Page(); // early return if required field is empty
                }

                response.FieldValues.Add(new WebPageFormFieldValue
                {
                    FieldId = field.Id,
                    Value = value
                });
            }

            _context.WebPageFormResponses.Add(response);
            await _context.SaveChangesAsync();

            TempData["FormSuccess"] = "Thank you! Your response has been submitted.";
            return RedirectToPage(new { slug = Slug });
        }

    }
}
