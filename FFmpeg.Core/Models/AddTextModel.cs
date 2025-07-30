using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{

    public class AddTextModel
    {
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public string Text { get; set; }
        public string FontColor { get; set; } = "white";
        public int FontSize { get; set; } = 24;
        public int PositionX { get; set; } = 100;
        public int PositionY { get; set; } = 50;
        public bool EnableAnimation { get; set; } = false;
    }
}