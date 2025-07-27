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
                .SetOutput(model.OutputFile)
                .AddOption($"-vf \"setpts={model.SpeedFactor}*PTS\"")
                .SetVideoCodec(model.VideoCodec);
            if (model.MaintainAudio)
            {
                CommandBuilder.AddOption($"-filter:a atempo={model.SpeedFactor}");
            }
            return await RunAsync();
        }
    }
}
