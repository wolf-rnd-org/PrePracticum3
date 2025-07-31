using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class ChangeSpeedModel
    {
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public double SpeedFactor { get; set; } // 0.5 = מהירות ×2, 2.0 = האטה ×0.5
        public string VideoCodec { get; set; } = "libx264";
    }
}

