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
    public class CreateGifCommand : BaseCommand, ICommand<CreateGifModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public CreateGifCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(CreateGifModel model)
        {
            int fps = model.Fps.HasValue && model.Fps > 0 ? model.Fps.Value : 10;
            int width = model.Width.HasValue && model.Width > 0 ? model.Width.Value : 320;
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption($"-vf fps={fps},scale={width}:-1")
                .SetOutput(model.OutputFile);

            return await RunAsync();
        }
    }
}
