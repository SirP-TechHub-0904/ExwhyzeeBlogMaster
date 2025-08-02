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
                .Where(c => c.IsPublished);

            //if (showInMenu.HasValue)
            //    query = query.Where(c => c.ShowInMenu == showInMenu.Value);

            //if (showInFooter.HasValue)
            //    query = query.Where(c => c.ShowInFooter == showInFooter.Value);

            //if (showInHome.HasValue)
            //    query = query.Where(c => c.ShowInHome == showInHome.Value);

            //if (showInHomeMain.HasValue)
            //    query = query.Where(c => c.ShowInHomeMain == showInHomeMain.Value);

            //if (showInHomeList.HasValue)
            //    query = query.Where(c => c.ShowInHomeList == showInHomeList.Value);

            return await query
                .OrderBy(c => c.SortOrder)
                .ToListAsync();
        }
    }
}
