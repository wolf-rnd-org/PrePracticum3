using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class FadeEffectModel
    {
        public string InputFilePath { get; set; }
        public int FadeInDurationSeconds { get; set; }
        public string OutputFilePath { get; set; }
    }
}
