using FFmpeg.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Infrastructure.Commands
{
    public class CreateGifCommand:BaseCommand,ICommand<CreateGifModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public WatermarkCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(CreateGifModel model)
        {
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption("-vf", $"fps={model.Fps},scale={model.Width}:-1")
                .SetOutput(model.OutputFile, true);

            return await RunAsync();
        }
    }
}
