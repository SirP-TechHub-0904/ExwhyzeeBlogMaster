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
    public interface IWebPageService
    {
        Task<List<WebPage>> GetPublishedPages();
    }
    public class WebPageService : IWebPageService
    {
        private readonly ApplicationDbContext _context;

        public WebPageService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<WebPage>> GetPublishedPages()
        {
            var query = _context.Pages
                .Where(p => p.IsPublished);

            
            return await query
                .OrderBy(p => p.SortOrder)
                .ToListAsync();
        }
    }

}
