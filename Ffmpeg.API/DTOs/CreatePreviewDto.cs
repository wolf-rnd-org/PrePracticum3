namespace FFmpeg.API.DTOs
{
    public class CreatePreviewDto
    {
        public IFormFile VideoFile { get; set; }        
        public string SeekTime { get; set; } = "00:00:05";
        public string OutputFileName { get; set; }
    }
}
