// FFmpeg.Core.Models/SubtitlesModel.cs
namespace FFmpeg.Core.Models
{
    public class SubtitlesModel
    {
        public string InputFile { get; set; }          // input.mp4
        public string SubtitleFile { get; set; }       // subtitles.srt
        public string OutputFile { get; set; }         // output.mp4
        public string VideoCodec { get; set; } = "libx264";
    }
}
