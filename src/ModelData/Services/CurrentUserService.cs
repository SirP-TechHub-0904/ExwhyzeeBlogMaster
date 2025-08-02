using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelData.Services
{
    public interface ICurrentUserService
    {
        string Username { get; }
        string FullName { get; }
    }

    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string Username =>
            _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

        public string FullName =>
            _httpContextAccessor.HttpContext?.User?.FindFirst("FullName")?.Value ?? "";
    }
}
