namespace FFmpeg.API.DTOs
{
    public class ChangeResolutionDto
    {
        public IFormFile VideoFile { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
