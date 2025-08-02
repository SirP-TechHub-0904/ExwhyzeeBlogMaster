using Microsoft.AspNetCore.Http;
using ModelData.Data;
using ModelData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelData.Services
{
    public interface ISystemLogService
    {
        Task LogAsync(string actionType, string? entityName, string? details, string? username = null, string? fullName = null);

        Task LogErrorAsync(Exception ex);
    }

    public class SystemLogService : ISystemLogService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SystemLogService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(string actionType, string? entityName, string? details, string? username = null, string? fullName = null)
        {
            var ip = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

            var log = new SystemLog
            {
                ActionType = actionType,
                EntityName = entityName,
                Details = details,
                Username = username,
                FullName = fullName,
                IpAddress = ip
            };

            _context.SystemLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task LogErrorAsync(Exception ex)
        {
            var log = new SystemLog
            {
                ActionType = "Error",
                EntityName = "System",
                Details = $"{ex.Message} | {ex.StackTrace}",
                Timestamp = DateTime.UtcNow
            };

            _context.SystemLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}