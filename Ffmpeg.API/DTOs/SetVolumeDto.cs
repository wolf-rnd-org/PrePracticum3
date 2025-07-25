namespace FFmpeg.API.DTOs
{
    public class SetVolumeDto
    {
        public IFormFile VideoFile { get; set; }
        public double Volume { get; set; } = 1.0;
    }
}