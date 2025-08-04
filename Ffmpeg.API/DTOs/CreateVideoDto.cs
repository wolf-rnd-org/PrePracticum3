namespace FFmpeg.API.DTOs
{
    public class CreateVideoDto
    {
        public List<IFormFile> ImageFiles { get; set; }
        public string OutputFile { get; set; }
    }
}
