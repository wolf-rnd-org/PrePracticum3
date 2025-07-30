namespace FFmpeg.API.DTOs
{
    public class CutVideoDto
    {
        public IFormFile VideoFile { get; set; }
        public string StartTime { get; set; } = "00:00:00";
        public string EndTime { get; set; } = "00:00:00";
    }
}
