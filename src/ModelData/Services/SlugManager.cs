using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModelData.Services
{
    public class SlugManager
    {
        public static string GenerateSlug(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return string.Empty;

            // Convert to lower case
            string slug = title.ToLowerInvariant();

            // Remove invalid characters (including apostrophes)
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");

            // Replace apostrophes with nothing, if desired
            slug = slug.Replace("'", "");

            // Convert multiple spaces into one dash
            slug = Regex.Replace(slug, @"\s+", "-").Trim('-');

            // Ensure no multiple hyphens
            slug = Regex.Replace(slug, @"-+", "-");

            return slug;
        }

        public static string StripHtml(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            string output = System.Text.RegularExpressions.Regex.Replace(input, "<.*?>", string.Empty);
            return output;
        }
        public static string StripEverything(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            // Step 1: Remove <script>, <style>, and similar blocks manually
            html = Regex.Replace(html, "<(script|style|noscript)[^>]*>.*?</\\1>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            // Step 2: Strip all remaining HTML tags
            html = Regex.Replace(html, "<.*?>", string.Empty);

            // Step 3: Decode HTML entities like &nbsp;, &amp;, etc.
            return System.Net.WebUtility.HtmlDecode(html).Trim();
        }

    }
}
