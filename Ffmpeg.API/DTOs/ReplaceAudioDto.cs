namespace FFmpeg.API.DTOs
{
    public class ReplaceAudioDto
    {
        public IFormFile VideoFile { get; set; }
        public IFormFile AudioFile { get; set; }
    }
}
