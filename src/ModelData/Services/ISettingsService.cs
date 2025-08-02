using ModelData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelData.Services
{
    public interface ISettingsService
    {
        Task<Setting?> GetSettingsAsync();
    }
}
