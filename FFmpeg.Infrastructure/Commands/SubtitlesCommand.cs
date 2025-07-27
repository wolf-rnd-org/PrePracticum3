// Ffmpeg.Command.Commands/SubtitlesCommand.cs
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Commands;
using FFmpeg.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace Ffmpeg.Command.Commands
{
    public class SubtitlesCommand : BaseCommand, ICommand<SubtitlesModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public SubtitlesCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(SubtitlesModel model)
        {
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption($"-vf subtitles={model.SubtitleFile}") // embed .srt
                .AddOption("-map 0:a?")
                .AddOption("-c:a copy")
                .SetVideoCodec(model.VideoCodec)
                .SetOutput(model.OutputFile, false);              // overwrite = false

            return await RunAsync();
        }
    }
}
