using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{

    public class AddTextModel
    {
        public string InputFile { get; set; } = string.Empty;
        public string OutputFile { get; set; } = string.Empty;
        public string Text { get; set; } = "Hello World";
        public string FontColor { get; set; } = "white";
        public int FontSize { get; set; } = 24;
        public int X { get; set; } = 100;
        public int Y { get; set; } = 50;
    }


}
