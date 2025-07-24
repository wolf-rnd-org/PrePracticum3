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
                string drawtextFilter = $"drawtext=text='%{{pts\\:hms}}':x={model.X}:y={model.Y}:fontsize={model.FontSize}:fontcolor={model.FontColor}";

                CommandBuilder = _commandBuilder
                    .SetInput(model.InputFile)
                    .AddFilterComplex(drawtextFilter)
                    .SetOutput(model.OutputFile);

                return await RunAsync();
            }
        }
    }
