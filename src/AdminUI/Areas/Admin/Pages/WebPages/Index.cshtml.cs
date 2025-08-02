using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelData.Data;
using ModelData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminUI.Areas_Admin_Pages_WebPages
{
    [Authorize(Policy = "WebPagesPolicy")]

    public class IndexModel : PageModel
    {
        private readonly ModelData.Data.ApplicationDbContext _context;

        public IndexModel(ModelData.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<WebPage> WebPage { get;set; } = default!;
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public string Search { get; set; } = "";
        public async Task OnGetAsync(string? search, int pageNumber = 1)
        {
            Search = search ?? "";
            CurrentPage = pageNumber < 1 ? 1 : pageNumber;
            int pageSize = 50;

            var query = _context.Pages
                .Include(search => search.Form)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(Search))
            {
                query = query.Where(p => p.Title.Contains(Search));
            }

            TotalCount = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)pageSize);

            WebPage = await query
                .OrderByDescending(p => p.Id)
                .Skip((CurrentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

    }
}
