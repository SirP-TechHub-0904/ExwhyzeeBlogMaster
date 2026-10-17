using Microsoft.AspNetCore.Hosting;
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
    public class UploadService
    {
        private readonly IWebHostEnvironment _env;
        private readonly long _maxImageSize;
        private readonly long _maxVideoSize;
        private readonly ApplicationDbContext _context;
        private readonly CloudinaryService _cloudinary;

        public UploadService(IWebHostEnvironment env, long maxImageSizeKB, long maxVideoSizeKB, ApplicationDbContext context, CloudinaryService? cloudinary = null)
        {
            _env = env;
            _maxImageSize = maxImageSizeKB * 1024;
            _maxVideoSize = maxVideoSizeKB * 1024;
            _context = context;
            _cloudinary = cloudinary ?? new CloudinaryService();
        }

        public async Task<(bool Success, string? FilePath, string? Error)> UploadMediaAsync(IFormFile file, bool isVideo = false)
        {
            if (file == null || file.Length == 0)
                return (false, null, "File is empty.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            isVideo = extension == ".mp4" || extension == ".mov" || extension == ".avi" || extension == ".mkv" || extension == ".webm";

            if (isVideo && file.Length > _maxVideoSize)
                return (false, null, $"Video size exceeds {_maxVideoSize / 1024}KB.");

            if (!isVideo && file.Length > _maxImageSize)
                return (false, null, $"Image size exceeds {_maxImageSize / 1024}KB.");

            // ✅ Upload to Cloudinary
            var uploadResult = await _cloudinary.UploadFileAsync(file);
            if (!uploadResult.Success || string.IsNullOrEmpty(uploadResult.Url))
            {
                return (false, null, uploadResult.Error ?? "Failed to upload file to Cloudinary.");
            }

            var remoteUrl = uploadResult.Url;
            var originalName = Path.GetFileNameWithoutExtension(file.FileName);

            // ✅ Save to database
            var media = new MediaFile
            {
                Url = remoteUrl,
                AltText = originalName ?? "altText",
                UploadedAt = DateTime.UtcNow
            };

            _context.MediaFiles.Add(media);
            await _context.SaveChangesAsync();

            return (true, remoteUrl, null);
        }
    }
}
