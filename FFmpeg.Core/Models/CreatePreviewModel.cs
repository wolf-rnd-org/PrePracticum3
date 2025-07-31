using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    internal class CreatePreviewModel
    {
        public string InputFile { get; set; }

        public string OutputImage { get; set; }

        public string Timestamp { get; set; } = "00:00:05";
    }
}
