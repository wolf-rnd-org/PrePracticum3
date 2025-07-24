namespace FFmpeg.API.DTOs
{
    public class ReplaceGreenScreenDto
    {
        public IFormFile VideoFile { get; set; }
        public IFormFile GreenScreenFile { get; set; }
        public string OutputVideoName { get; set; } = "output.mp4";
    }
}
