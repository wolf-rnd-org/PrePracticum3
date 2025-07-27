﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class CreateGifModel
    {
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public int Fps { get; set; } = 10;
        public int YPosition { get; set; }
        public int Width { get; set; } = 320;

    }
}
