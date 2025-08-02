namespace ModelData.Models
{
    public class Setting
    {
        public int Id { get; set; }

        // === Basic Site Info ===
        public string? SiteTitle { get; set; }
        public string? SiteDescription { get; set; }
        public string? SiteTagline { get; set; }
        public string? SiteLogoUrl { get; set; }
        public string? FaviconUrl { get; set; }

        // === SEO & Meta ===
        public string? MetaKeywords { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaAuthor { get; set; }
        public string? MetaHeaderCode { get; set; } // e.g. extra <meta> tags in <head>

        // === Contact / Info ===
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? Address { get; set; }

        // === Custom Code Injections ===
        public string? CustomCSS { get; set; }
        public string? CustomJS { get; set; }
        public string? CustomHeadScript { get; set; }
        public string? CustomFooterScript { get; set; }

        // === Social Media Handles ===
        public string? FacebookUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? YouTubeUrl { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? TikTokUrl { get; set; }

        // === Social Media Handles ===
        public string? FacebookWidget { get; set; }
        public string? TwitterWidget { get; set; }
        public string? InstagramWidget { get; set; }
        public string? YouTubeWidget { get; set; }
        public string? LinkedInWidget { get; set; }
        public string? TikTokWidget { get; set; }


        public bool EnableLiveChat { get; set; }
        public string? LiveChatScript { get; set; }  // e.g. Tawk.to, Intercom, etc.

        // === Analytics & Tracking ===
        public string? GoogleAnalyticsId { get; set; }
        public string? GoogleTagManagerId { get; set; }
        public string? FacebookPixelId { get; set; }



        public string? GoogleAnalyticsIframeWidget { get; set; }

        public long MaxImageSizeKB { get; set; } = 500; // default to 500KB
        public long MaxVideoSizeKB { get; set; } = 10240; // default to 10MB


        public string? SiteBreadcrumbImageUrl { get; set; }

        // === Maintenance Mode ===
        public bool IsMaintenanceMode { get; set; }
        public string? MaintenanceMessage { get; set; }
    }


}
