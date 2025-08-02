using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelData.Models
{
    public class SystemLog
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? FullName { get; set; }
        public string ActionType { get; set; } // Login, Create, Update, Delete, Error
        public string? EntityName { get; set; } // e.g. Post, Category, User
        public string? Details { get; set; } // e.g. Title deleted, Error message, etc.
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? IpAddress { get; set; }
    }
}
