using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelData.Models
{
    public class NewsletterSubscription
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;
    }
}
