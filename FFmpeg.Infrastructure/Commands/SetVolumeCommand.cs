
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
    public class SetVolumeCommand : BaseCommand, ICommand<SetVolumeModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public SetVolumeCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(SetVolumeModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (model.Volume <= 0) throw new ArgumentException("Volume must be greater than 0");
            CommandBuilder = _commandBuilder
                    .SetInput(model.InputFile)
                    .AddOption($"-af volume={model.Volume}")
                    .SetOutput(model.OutputFile);

            return await RunAsync();
        }
    }
}
