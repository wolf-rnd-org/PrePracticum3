using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ffmpeg.Command.Commands;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;

namespace FFmpeg.Infrastructure.Commands
{
        public class ChangeResolutionCommand : BaseCommand, ICommand<ChangeResolutionModel>
        {
            private readonly ICommandBuilder _commandBuilder;

            public ChangeResolutionCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder) : base(executor)
            {
                _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
            }

            public async Task<CommandResult> ExecuteAsync(ChangeResolutionModel model)
            {
                CommandBuilder = _commandBuilder
                    .SetInput(model.InputFile)
                    .AddOption($"-vf scale={model.Width}:{model.Height}")
                    .SetVideoCodec(model.VideoCodec)
                    .SetOutput(model.OutputFile, false);

                return await RunAsync();
            }
        }
}

