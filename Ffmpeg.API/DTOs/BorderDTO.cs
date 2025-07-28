namespace FFmpeg.API.DTOs
{
    public class BorderDTO
    {
        public IFormFile VideoFile { get; set; }
        public string? BorderSize { get; set; } = "20";
        public string? FrameColor { get; set; } = "blue";
    }
}
