using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class TimestampModel
    {
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public int X { get; set; } = 10;
        public int Y { get; set; } = 10;
        public int FontSize { get; set; } = 24;
        public string FontColor { get; set; } = "white";
    }
}
