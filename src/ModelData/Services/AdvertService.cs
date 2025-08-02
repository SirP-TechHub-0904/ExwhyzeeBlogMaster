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
     public interface IAdvertService
    {
        Task<List<Advert>> GetAdvertsAsync();

        Task<Advert?> GetAdvertByPositionAsync(AdvertPosition position);

    }


    public class AdvertService : IAdvertService
    {
        private readonly ApplicationDbContext _context;

        public AdvertService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Advert?> GetAdvertByPositionAsync(AdvertPosition position)
        {
            return await _context.Adverts
                .Where(a => a.Position == position)
                .OrderByDescending(a => a.Id) // In case there are multiple, get latest
                .FirstOrDefaultAsync();
        }


        public async Task<List<Advert>> GetAdvertsAsync()
        {
             


            var query = _context.Adverts ;
            return await query 
                .ToListAsync();
        }
    }
}
