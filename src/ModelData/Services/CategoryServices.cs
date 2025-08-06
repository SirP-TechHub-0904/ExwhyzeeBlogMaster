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
     public interface ICategoryServices
        {
            Task<List<Category>> GetCategoriesAsync();
        }
     

    public class CategoryServices : ICategoryServices
    {
        private readonly ApplicationDbContext _context;

        public CategoryServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            var query = _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.Children)
                .Include(c => c.Posts)
                .Where(c => c.IsPublished)
                .Select(c => new
                {
                    Category = c,
                    LatestPostDate = c.Posts
                        .Where(p => p.IsPublished)
                        .Max(p => (DateTime?)p.PublishedAt) ?? DateTime.MinValue
                });

            var result = await query
                .OrderByDescending(x => x.LatestPostDate)
                .Select(x => x.Category)
                .ToListAsync();

            return result;
        }

    }
}
