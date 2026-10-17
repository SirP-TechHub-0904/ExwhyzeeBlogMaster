using System;
using System.IO;

namespace ModelData
{
    public static class EnvLoader
    {
        /// <summary>
        /// Loads key-value pairs from a .env file into the process environment variables if present.
        /// </summary>
        public static void Load(string? filePath = null)
        {
            try
            {
                filePath ??= Path.Combine(Directory.GetCurrentDirectory(), ".env");
                if (!File.Exists(filePath))
                {
                    // Also check parent directory if running from bin/Debug or subfolder
                    var parentPath = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
                    if (File.Exists(parentPath))
                    {
                        filePath = parentPath;
                    }
                    else
                    {
                        return;
                    }
                }

                foreach (var line in File.ReadAllLines(filePath))
                {
                    var trimmed = line.Trim();
                    if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#"))
                        continue;

                    var separatorIndex = trimmed.IndexOf('=');
                    if (separatorIndex <= 0)
                        continue;

                    var key = trimmed.Substring(0, separatorIndex).Trim();
                    var value = trimmed.Substring(separatorIndex + 1).Trim();

                    // Strip wrapping double or single quotes if present
                    if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                        (value.StartsWith("'") && value.EndsWith("'")))
                    {
                        if (value.Length >= 2)
                        {
                            value = value.Substring(1, value.Length - 2);
                        }
                    }

                    if (!string.IsNullOrEmpty(key))
                    {
                        Environment.SetEnvironmentVariable(key, value);
                    }
                }
            }
            catch
            {
                // Silently ignore if file cannot be read so normal startup is uninterrupted
            }
        }
    }
}
