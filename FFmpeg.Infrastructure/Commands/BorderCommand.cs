using Ffmpeg.Command.Commands;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Commands;
using FFmpeg.Infrastructure.Services;

public class BorderCommand : BaseCommand, ICommand<BorderModel>
{
    private readonly ICommandBuilder _commandBuilder;

    public BorderCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
        : base(executor)
    {
        _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
    }

    public async Task<CommandResult> ExecuteAsync(BorderModel model)
    {
        if (string.IsNullOrWhiteSpace(model.VideoName) ||
            string.IsNullOrWhiteSpace(model.VideoOutputName) ||
            string.IsNullOrWhiteSpace(model.FrameColor))
        {
            throw new ArgumentException("Video name, frame color, and video output name are required.");
        }

        CommandBuilder = _commandBuilder
            .SetInput(model.VideoName)
            .AddOption($"-vf pad=width=iw+40:height=ih+40:x=20:y=20:color={model.FrameColor}")
            .SetOutput(model.VideoOutputName);

        return await RunAsync();
    }
}
