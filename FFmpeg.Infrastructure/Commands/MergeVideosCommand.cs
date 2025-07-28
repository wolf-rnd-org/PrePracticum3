using Ffmpeg.Command.Commands;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;
using System;
using System.Threading.Tasks;
using FFmpeg.Core.Interfaces;

namespace FFmpeg.Infrastructure.Commands
{
    public class MergeVideosCommand : BaseCommand, ICommand<MergeVideosModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public MergeVideosCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(MergeVideosModel model)
        {
            string filter = model.Mode == "vertical" ? "vstack=inputs=2" : "hstack=inputs=2";

            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile1)
                .SetInput(model.InputFile2)
                .AddOption($"-filter_complex \"[0:v]scale=640:360[v0];[1:v]scale=640:360[v1];[v0][v1]{filter}\"")
                .SetOutput(model.OutputFile);

            return await RunAsync();
        }
    }
}
