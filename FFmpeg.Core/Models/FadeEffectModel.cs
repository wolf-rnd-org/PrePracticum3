using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class FadeEffectModel
    {
        public string InputPath { get; set; }
        public string OutputPath { get; set; }
        public double Duration { get; set; }
    }
}
