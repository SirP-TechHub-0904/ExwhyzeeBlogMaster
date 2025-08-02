using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelData.Models
{
    public class PostImage
    {
        public int Id { get; set; }

        public int? PostId { get; set; }
        public Post Post { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;

        public bool IsDefault { get; set; } = false;
    }
}
