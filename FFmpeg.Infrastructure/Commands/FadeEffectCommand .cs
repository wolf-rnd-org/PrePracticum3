using Ffmpeg.Command.Commands;
using FFmpeg.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Infrastructure.Commands
{
    public class FadeEffectCommand
    {
        public class FadeEffectCommand : BaseCommand, ICommand<FadeEffectModel>
        {
            private readonly ICommandBuilder _commandBuilder;

            public FadeEffectCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
                : base(executor)
            {
                _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
            }

            public async Task<CommandResult> ExecuteAsync(FadeEffectModel model)
            {
                if (model == null) throw new ArgumentNullException(nameof(model));
                if (string.IsNullOrEmpty(model.InputFilePath)) throw new ArgumentException("Input file required");
                if (string.IsNullOrEmpty(model.OutputFilePath)) throw new ArgumentException("Output file required");

                string fadeFilter = $"fade=t=in:st=0:d={model.FadeInDurationSeconds}";

                CommandBuilder = _commandBuilder
                    .SetInput(model.InputFilePath)
                    .AddOption($"-vf {fadeFilter}")
                    .SetVideoCodec("libx264")
                    .SetOutput(model.OutputFilePath);

                return await RunAsync();
            }
        }
    }
}
