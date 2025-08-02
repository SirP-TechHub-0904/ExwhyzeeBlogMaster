using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelData.Data;
using ModelData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminUI.Controllers
{

    [Route("api/universals")]
    [ApiController]
    [AllowAnonymous] // Allow anonymous access for universals like newsletter subscription
    public class UniversalsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UniversalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("toggle-approval/{id}")]
        public async Task<IActionResult> ToggleApproval(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null) return NotFound();

            comment.IsApproved = !comment.IsApproved;
            await _context.SaveChangesAsync();

            return Ok(new { success = true, isApproved = comment.IsApproved });
        }

        [HttpPost("subscribe-newsletter")]
        [AllowAnonymous]
        public async Task<IActionResult> SubscribeNewsletter([FromBody] NewsletterSubscription model)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
                return BadRequest(new { success = false, message = "Email is required." });

            if (await _context.NewsletterSubscriptions.AnyAsync(x => x.Email == model.Email))
                return BadRequest(new { success = false, message = "Email already subscribed." });

            model.SubscribedAt = DateTime.UtcNow;
            _context.NewsletterSubscriptions.Add(model);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Subscription successful!" });
        }

    }
}
