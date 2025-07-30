namespace FFmpeg.API.DTOs
{
    public class ReplaceGreenScreenDto
    {
        public IFormFile VideoFile { get; set; }
        public IFormFile BackgroundFile { get; set; }
    }
}
