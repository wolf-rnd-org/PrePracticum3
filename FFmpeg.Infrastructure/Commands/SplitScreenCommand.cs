using Ffmpeg.Command.Commands;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Commands;
using FFmpeg.Infrastructure.Services;

public class SplitScreenCommand : BaseCommand, ICommand<SplitScreenModel>
{
    private readonly ICommandBuilder _commandBuilder;

    public SplitScreenCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
        : base(executor)
    {
        _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
    }

    public async Task<CommandResult> ExecuteAsync(SplitScreenModel model)
    {
        if (model.DuplicateCount < 2)
            throw new ArgumentException("Duplicate count must be at least 2");

        string filter = BuildSplitHstackFilter(model.DuplicateCount);

        CommandBuilder = _commandBuilder
            .SetInput(model.InputFile)
            .AddFilterComplex(filter) // הפילטר כולל את [out]
            .SetVideoCodec(model.VideoCodec)
            .SetOutput(model.OutputFile); // FFmpeg יזהה [out] כברירת מחדל

        return await RunAsync();
    }

    private static string BuildSplitHstackFilter(int count)
    {
        var splitLabels = Enumerable.Range(0, count).Select(i => $"v{i}").ToList();
        string split = $"[0:v]split={count}" + string.Concat(splitLabels.Select(l => $"[{l}]")) + ";";

        string hstackChain = "";
        string lastLabel = $"[{splitLabels[0]}]";
        for (int i = 1; i < count; i++)
        {
            string tmpLabel = (i == count - 1) ? "[out]" : $"[tmp{i}]";
            hstackChain += $"{lastLabel}[{splitLabels[i]}]hstack=inputs=2{tmpLabel};";
            lastLabel = tmpLabel;
        }

        return split + hstackChain.TrimEnd(';'); // נגמר עם [out]
    }
}
