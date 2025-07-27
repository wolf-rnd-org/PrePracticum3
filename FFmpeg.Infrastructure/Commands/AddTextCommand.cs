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
            if (string.IsNullOrWhiteSpace(model.InputFile) || string.IsNullOrWhiteSpace(model.OutputFile))
            {
                return new CommandResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Input and output files must be provided."
                };
            }

            string drawTextFilter = $"drawtext=text='{model.Text}':" +
                                    $"x={model.X}:y={model.Y}:" +
                                    $"fontsize={model.FontSize}:" +
                                    $"fontcolor={model.FontColor}";

            _commandBuilder
                .SetInput(model.InputFile)
                .AddFilterComplex(drawTextFilter)
                .SetOutput(model.OutputFile);

            return await RunAsync();
        }
    }
}
