using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public  class ConvertFormatRequest
    {
        public string InputFileName { get; set; } = string.Empty;
        public string OutputFileName { get; set; } = string.Empty;
    }
}
