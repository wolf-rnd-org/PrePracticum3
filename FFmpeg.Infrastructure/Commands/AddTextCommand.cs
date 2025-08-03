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
    public class AddTextCommand : BaseCommand, ICommand<AddTextModel>
    {
        private readonly ICommandBuilder _commandBuilder;
        public AddTextCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }
        public async Task<CommandResult> ExecuteAsync(AddTextModel model)
        {
            string animationX = model.EnableAnimation
                ? "x=w-t*100" // תזוזה מימין לשמאל – אפשר לשנות
                : $"x={model.PositionX}";
            string animationY = $"y={model.PositionY}";
            string drawTextFilter = $"drawtext=text='{model.Text}':{animationX}:{animationY}:fontsize={model.FontSize}:fontcolor={model.FontColor}";
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption($"-vf \"{drawTextFilter}\"")
                .SetOutput(model.OutputFile);
            return await RunAsync();
        }
    }
}