﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class ReplaceGreenScreenModal
    {
        public string InputVideoName { get; set; }
        public string BackgroundVideoName { get; set; }
        public string OutputVideoName { get; set; }
    }
}
