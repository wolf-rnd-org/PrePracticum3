using Ffmpeg.Command.Commands;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Infrastructure.Commands
{
    public class CreateVideoCommand : BaseCommand, ICommand<CreateVideoModel>
    {
        private readonly ICommandBuilder _commandBuilder;


        public CreateVideoCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(CreateVideoModel model)
        {
            CommandBuilder = _commandBuilder.SetInput(string.Join("|", model.ImageSequence))
                .SetOutput(model.OutputFile)
                .AddOption($"-framerate {model.FrameRate}");

            return await RunAsync();
        }
    }
}
