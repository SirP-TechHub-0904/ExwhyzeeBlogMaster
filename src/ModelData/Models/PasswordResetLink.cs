using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelData.Models
{
    public class PasswordResetLink
    {
        public int Id { get; set; }
        public string ShortCode { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 8);
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
    }

}
