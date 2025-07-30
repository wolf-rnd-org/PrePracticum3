using Ffmpeg.Command.Commands;
using FFmpeg.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Infrastructure.Commands
{
    public class ConvertFormatCommand : BaseCommand
    {
        public string InputFile { get; }
        public string OutputFile { get; }

        public ConvertFormatCommand(FFmpegExecutor executor, string inputFile, string outputFile)
            : base(executor)
        {
            InputFile = inputFile;
            OutputFile = outputFile;
        }

        public  string BuildCommand()
        {
            return $"-i \"{InputFile}\" \"{OutputFile}\"";
        }
    }

}
