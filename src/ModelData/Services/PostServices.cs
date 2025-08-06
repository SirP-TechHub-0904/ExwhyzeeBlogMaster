using Microsoft.EntityFrameworkCore;
using ModelData.Data;
using ModelData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelData.Services
{
    public interface IPostServices
    {
        Task<List<Post>> GetPublishedPosts(
            bool? isFeatured = null,
            bool? showInHero = null,
            bool? showInSurface = null,
            bool? commentsEnabled = null);

        Task<List<Post>> GetTopViewedPostsAsync(int count = 10);
        Task<List<Post>> GetBrakingNews(int count = 10);
        Task<List<Post>> GetPostsByCategory(int categoryId);
        Task<List<Post>> GetRecentPosts();
        Task<Post?> GetPostBySlugAsync(string slug);
        Task<Post?> GetPreviousPostAsync(DateTime currentPostDate);
        Task<Post?> GetNextPostAsync(DateTime currentPostDate);

        Task<List<Comment>> GetApprovedCommentsByPostIdAsync(int postId);
        Task AddCommentAsync(Comment comment);

         
    }


    public class PostServices : IPostServices
    {
        private readonly ApplicationDbContext _context;

       
        public PostServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Post>> GetBrakingNews(int count = 10)
        {
            return await _context.Posts                
                .Where(p => p.IsPublished)  
                .Where(p => p.IsBreakingNews) 
                .OrderByDescending(p => p.PublishedAt) // Sort by most viewed
                .Take(count) // Limit to top N
                .ToListAsync();
        }

        public async Task<Post?> GetPostBySlugAsync(string slug)
        {
            return await _context.Posts
                .Include(p => p.Category)
                .Include(p => p.PostImages)
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Slug == slug && p.IsPublished);
        }


        public async Task<List<Post>> GetPostsByCategory(int categoryId)
        {
            var data = await _context.Posts
                .Include(x=>x.PostImages)
                    .Where(p => p.CategoryId == categoryId)
                    .OrderByDescending(p => p.PublishedAt)
                    .ToListAsync();
            return data;
        }

        public async Task<List<Post>> GetPublishedPosts(
            bool? isFeatured = null,
            bool? showInHero = null,
            bool? showInSurface = null,
            bool? commentsEnabled = null)
        {
            var query = _context.Posts
                .Include(p => p.Category)
                .Include(p => p.PostImages)
                .Where(p => p.IsPublished);

            if (isFeatured.HasValue)
                query = query.Where(p => p.IsFeatured == isFeatured.Value);

            if (showInHero.HasValue)
                query = query.Where(p => p.ShowInHero == showInHero.Value);

            if (showInSurface.HasValue)
                query = query.Where(p => p.ShowInSurface == showInSurface.Value);

            if (commentsEnabled.HasValue)
                query = query.Where(p => p.CommentsEnabled == commentsEnabled.Value);

            return await query
                .OrderBy(p => p.SortOrder).ToListAsync();
        }

        public async Task<List<Post>> GetRecentPosts()
        {
            var data = await _context.Posts
                .Include(x => x.PostImages)
                    .OrderByDescending(p => p.PublishedAt)
                    .ToListAsync();
            return data;
        }

        public async Task<List<Post>> GetTopViewedPostsAsync(int count = 10)
        {
            return await _context.Posts
                .Include(p => p.Category)
                .Include(p => p.PostImages)
                .Where(p => p.IsPublished) // Only published posts
                .OrderByDescending(p => p.ViewCount) // Sort by most viewed
                .Take(count) // Limit to top N
                .ToListAsync();
        }

        public async Task<Post?> GetPreviousPostAsync(DateTime currentPostDate)
        {
            return await _context.Posts
                                .Include(x => x.Category)
                                .Include(x => x.PostImages)

                .Where(p => p.IsPublished && (p.PublishedAt ?? p.Date) < currentPostDate)
                .OrderByDescending(p => p.PublishedAt ?? p.Date)
                .FirstOrDefaultAsync();
        }

        public async Task<Post?> GetNextPostAsync(DateTime currentPostDate)
        {
            return await _context.Posts
                .Include(x=>x.Category)
                .Include(x=>x.PostImages)
                .Where(p => p.IsPublished && (p.PublishedAt ?? p.Date) > currentPostDate)
                .OrderBy(p => p.PublishedAt ?? p.Date)
                .FirstOrDefaultAsync();
        }
        public async Task<List<Comment>> GetApprovedCommentsByPostIdAsync(int postId)
        {
            return await _context.Comments
                .Where(c => c.PostId == postId && c.IsApproved)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task AddCommentAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }
    }

}
