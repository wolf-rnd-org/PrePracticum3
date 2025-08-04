using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class CreateVideoModel
    {
        public List<string> ImageSequence { get; set; }
        public string OutputFile { get; set; }
        public int FrameRate { get; set; } = 30;
    }
}
