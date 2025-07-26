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
    public class ExtractFrameCommand : BaseCommand, ICommand<FrameExtractionModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public ExtractFrameCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(FrameExtractionModel model)
        {
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption($"-ss {model.TimeStamp}") // Seek to desired timestamp
                .AddOption("-vframes 1");            // Extract only 1 frame

            CommandBuilder.SetOutput(model.OutputImage, true);

            return await RunAsync();
        }
    }
}
