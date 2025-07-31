namespace FFmpeg.API.DTOs
{
    public class ChangeSpeedDto
    {
        public IFormFile VideoFile { get; set; }
        public double UserSpeedFactor { get; set; } = 2.0;
    }
}
