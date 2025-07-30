using Ffmpeg.Command.Commands;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Infrastructure.Commands
{
        public class TimestampCommand : BaseCommand, ICommand<TimestampModel>
        {
            private readonly ICommandBuilder _commandBuilder;

            public TimestampCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
                : base(executor)
            {
                _commandBuilder = commandBuilder;
            }

        public async Task<CommandResult> ExecuteAsync(TimestampModel model)
        {
            string drawtextFilter = $"drawtext=fontfile=/Windows/Fonts/arial.ttf:text='%{{pts\\:hms}}':x={model.X}:y={model.Y}:fontsize={model.FontSize}:fontcolor={model.FontColor}";

            CommandBuilder = _commandBuilder
               .SetInput(model.InputFile)
               .AddOption($"-vf \"{drawtextFilter}\"")
               .AddOption("-map 0:v")     
               .AddOption("-map 0:a?")
               .AddOption("-c:a copy")
               .SetVideoCodec("libx264")
               .SetOutput(model.OutputFile);

            return await RunAsync();
        }

    }
}
