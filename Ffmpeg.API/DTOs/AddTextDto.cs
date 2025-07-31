namespace FFmpeg.API.DTOs
{
    public class AddTextDto
    {
        public IFormFile VideoFile { get; set; }
        public string Text { get; set; }
        public string FontColor { get; set; } = "white";
        public int FontSize { get; set; } = 24;
        public int XPosition { get; set; } = 100;
        public int YPosition { get; set; } = 50;
        public bool EnableAnimation { get; set; } = false;
    }
}
