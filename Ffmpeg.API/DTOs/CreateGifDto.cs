using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class CreateGifDto
    {
        public IFormFile InputFile { get; set; }
        public int Fps { get; set; } = 10;
        public int Width { get; set; } = 320;
    }
}
