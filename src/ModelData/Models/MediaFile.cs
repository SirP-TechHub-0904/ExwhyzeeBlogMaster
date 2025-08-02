namespace ModelData.Models
{

    public class MediaFile
    {
        public int Id { get; set; }
        public string Url { get; set; } = null!;
        public string? AltText { get; set; }
        public DateTime UploadedAt { get; set; }
 
    }

}
