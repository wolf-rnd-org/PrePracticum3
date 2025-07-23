using Ffmpeg.Command.Commands;
using FFmpeg.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Infrastructure.Commands
{
    public class ReverseVideoCommand:BaseCommand,ICommand<ReverseVideoModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public WatermarkCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }
        public async Task<CommandResult> ExecuteAsync(ReverseVideoModel model)
        {
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption($"-vf reverse")
                .SetInput(model.OutputFile);
        

            return await RunAsync();
        }
    }
}
