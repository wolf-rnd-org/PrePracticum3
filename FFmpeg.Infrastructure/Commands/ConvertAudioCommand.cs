using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Commands;
using FFmpeg.Infrastructure.Services;

namespace Ffmpeg.Command.Commands
{
    public class ConvertAudioCommand : BaseCommand, ICommand<ConvertAudioModel>
    {
        private readonly ICommandBuilder _builder;
        public ConvertAudioCommand(FFmpegExecutor executor, ICommandBuilder builder)
            : base(executor)
        {
            _builder = builder;
        }
        public async Task<CommandResult> ExecuteAsync(ConvertAudioModel model)
        {
            CommandBuilder = _builder
                .SetInput(model.InputFile)
                .SetOutput(model.OutputFile);
            return await RunAsync();
        }
    }
}