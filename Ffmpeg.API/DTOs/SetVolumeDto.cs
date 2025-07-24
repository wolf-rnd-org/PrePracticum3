namespace FFmpeg.API.DTOs
{
    public class SetVolumeDto
    {
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public double Volume { get; set; }
    }
}