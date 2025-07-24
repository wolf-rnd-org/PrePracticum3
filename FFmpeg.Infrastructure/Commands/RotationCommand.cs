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
    public class RotationCommand : BaseCommand, ICommand<RotationModel>
    {
        private readonly ICommandBuilder _commandBuilder;
        public RotationCommand(FFmpegExecutor executor,ICommandBuilder commandBuilder) : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(RotationModel model)
        {
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile);

            if (model.Angle != 0)
            {
                double radians = model.Angle * Math.PI / 180;
                CommandBuilder.AddFilterComplex($"[0:v]rotate={radians}[out]");
            }

            CommandBuilder
                .AddOption("-map 0:a?")
                .AddOption("-c:a copy");

            CommandBuilder.SetOutput(model.OutputFile);

            return await RunAsync();
        }
    }
}
