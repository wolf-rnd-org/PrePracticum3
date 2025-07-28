namespace FFmpeg.API.DTOs
{
    public class MergeVideosDto
    {
        public IFormFile VideoFile1 { get; set; }
        public IFormFile VideoFile2 { get; set; }
        public string Mode { get; set; } // "horizontal" או "vertical"
    }
}
