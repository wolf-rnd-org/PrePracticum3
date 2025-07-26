using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class FrameExtractionModel
    {
            public string InputFile { get; set; } 
            public string TimeStamp { get; set; } 
            public string OutputImage { get; set; }
    }
}
