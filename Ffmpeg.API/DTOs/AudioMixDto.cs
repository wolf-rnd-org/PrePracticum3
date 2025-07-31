namespace FFmpeg.API.DTOs
{
    public class AudioMixDto
    {
        public IFormFile InputFile1 { get; set; }
        public IFormFile InputFile2 { get; set; }
        public string OutputFileName { get; set; }
    }
}
