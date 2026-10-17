using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ModelData.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        public const string DefaultCloudinaryUrl = "cloudinary://825271523643292:n2owFIA8dFsQxToBH0_nEJKMl6Y@yffz4ii9";

        public CloudinaryService(IConfiguration? config = null)
        {
            var cloudinaryUrl = config?["CLOUDINARY_URL"]
                ?? Environment.GetEnvironmentVariable("CLOUDINARY_URL")
                ?? DefaultCloudinaryUrl;

            _cloudinary = new Cloudinary(cloudinaryUrl);
            _cloudinary.Api.Secure = true;
        }

        public async Task<(bool Success, string? Url, string? Error)> UploadFileAsync(IFormFile file, string folder = "exwhyzee_blogs")
        {
            if (file == null || file.Length == 0)
                return (false, null, "File is empty.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            bool isVideo = extension == ".mp4" || extension == ".mov" || extension == ".avi" || extension == ".mkv" || extension == ".webm";

            using var stream = file.OpenReadStream();
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(file.FileName);
            var publicId = $"{fileNameWithoutExt}-{DateTime.UtcNow.Ticks}";

            if (isVideo)
            {
                var uploadParams = new VideoUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = folder,
                    PublicId = publicId
                };
                var result = await _cloudinary.UploadAsync(uploadParams);
                if (result.Error != null)
                    return (false, null, result.Error.Message);
                return (true, result.SecureUrl?.ToString() ?? result.Url?.ToString(), null);
            }
            else
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = folder,
                    PublicId = publicId
                };
                var result = await _cloudinary.UploadAsync(uploadParams);
                if (result.Error != null)
                    return (false, null, result.Error.Message);
                return (true, result.SecureUrl?.ToString() ?? result.Url?.ToString(), null);
            }
        }

        public async Task<(bool Success, string? Url, string? Error)> UploadLocalFileAsync(string localFilePath, string folder = "exwhyzee_blogs")
        {
            if (!File.Exists(localFilePath))
                return (false, null, "Local file not found.");

            var extension = Path.GetExtension(localFilePath).ToLowerInvariant();
            bool isVideo = extension == ".mp4" || extension == ".mov" || extension == ".avi" || extension == ".mkv" || extension == ".webm";

            using var stream = File.OpenRead(localFilePath);
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(localFilePath);

            if (isVideo)
            {
                var uploadParams = new VideoUploadParams
                {
                    File = new FileDescription(Path.GetFileName(localFilePath), stream),
                    Folder = folder,
                    PublicId = fileNameWithoutExt
                };
                var result = await _cloudinary.UploadAsync(uploadParams);
                if (result.Error != null)
                    return (false, null, result.Error.Message);
                return (true, result.SecureUrl?.ToString() ?? result.Url?.ToString(), null);
            }
            else
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(Path.GetFileName(localFilePath), stream),
                    Folder = folder,
                    PublicId = fileNameWithoutExt
                };
                var result = await _cloudinary.UploadAsync(uploadParams);
                if (result.Error != null)
                    return (false, null, result.Error.Message);
                return (true, result.SecureUrl?.ToString() ?? result.Url?.ToString(), null);
            }
        }
    }
}
