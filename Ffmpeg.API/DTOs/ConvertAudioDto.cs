using Microsoft.AspNetCore.Http;

namespace FFmpeg.API.DTOs
{
    public class ConvertAudioDto
    {
        public IFormFile AudioFile { get; set; }
        public string OutputFileName { get; set; }
    }
}
