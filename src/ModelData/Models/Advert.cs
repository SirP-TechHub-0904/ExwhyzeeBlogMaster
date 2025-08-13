using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelData.Models
{
    public class Advert
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public string Link { get; set; } = null!;
        public AdvertPosition Position { get; set; } // Enum for placement
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum AdvertPosition
    {
        Header,
        SidebarTop,
        SidebarBottom,
        Footer,
        HeroSection,
        BetweenPosts,
        MainTop,
        Middle

    }
}
