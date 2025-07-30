namespace FFmpeg.API.DTOs
{
    public class RotationDto
    {
        public IFormFile InputFile { get; set; }
        public double Angle { get; set; }
    }
}
