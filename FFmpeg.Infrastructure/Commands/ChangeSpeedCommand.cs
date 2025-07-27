using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Commands;
using FFmpeg.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ffmpeg.Command.Commands
{
    public class ChangeSpeedCommand : BaseCommand, ICommand<ChangeSpeedModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public ChangeSpeedCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(ChangeSpeedModel model)
        {
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption($"-map 0:a?")
                .AddOption($"-c:a copy");

            CommandBuilder.SetOutput(model.OutputFile);

            CommandBuilder = _commandBuilder
               .SetInput(model.InputFile)
               .AddFilterComplex($"setpts={model.SpeedFactor}*PTS")
               .SetVideoCodec(model.VideoCodec);

            if (model.MaintainAudio)
            {
                double audioTempo = 1.0 / model.SpeedFactor;
                CommandBuilder.AddOption($"-filter:a atempo={audioTempo}");
            }

            CommandBuilder.SetOutput(model.OutputFile);

            return await RunAsync();
        }
    }
}
